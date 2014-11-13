package com.inspiracode.praxma.rhi.jasper.customizer;
import java.awt.Color;
import java.awt.Font;
import java.util.HashMap;
import java.util.Map;

import net.sf.jasperreports.engine.JRChart;
import net.sf.jasperreports.engine.JRChartCustomizer;
import net.sf.jasperreports.engine.JRPropertiesMap;
import net.sf.jasperreports.engine.util.JRColorUtil;

import org.apache.commons.logging.Log;
import org.apache.commons.logging.LogFactory;
import org.jfree.chart.JFreeChart;
import org.jfree.chart.labels.StandardPieSectionLabelGenerator;
import org.jfree.chart.plot.PiePlot;
import org.jfree.chart.plot.Plot;

/**
 * @author mdahlman
 * This chart customizer allows the report developer to set the following JFreeChart properties
 * which are not otherwise set-able in iReport:
 * 
 *   Label Font
 *   Interior Gap
 *   Maximum Label Width
 *   Defining a pie slice's color based on key (rather than setting it based on the order they appear in)
 * 
 * This property applies only to Pie plots.
 * Charts that do not use pie plots will ignore these properties.
 * The properties should be set using the "Properties expressions" field in iReport. 
 * This results in .jrxml like the following:
 *   <hr/>
 *   <b>Change by Diego Torres:</b><br/>
 *   <property name="UseLabels" value="false"/>
 *   <property name="InnerLabel" value="false"/>
 *   <hr/>
 *   <property name="LabelFontName" value="Verdana"/>
 *   <property name="LabelFontStyle" value="0"/>
 *   <propertyExpression name="LabelFontStyle"><![CDATA[java.awt.Font.PLAIN]]></propertyExpression> <!-- use a propertyExpression to allow constants -->
 *   <property name="LabelFontSize" value="8"/>
 *   <property name="InteriorGap" value="0.02"/>
 *   <property name="MaximumLabelWidth" value="0.28"/>
 *   <property name="PredefinedColors" value="MySQL:#015A84;Oracle:red;PostgreSQL:#0085B0;Microsoft SQL Server:#F8EB33"/>
 *   
 *   The property PredefinedColors must be in this form:
 *   	key-expression1:color1;key-expression2:color2
 *   The key-expression is any string representing a section of the pie chart. It will be ignored if it does not match a key from the dataset exactly.
 *   The color must be in the form #000000 representing an RGB value of 3 hex values preceded by the hash sign,
 *   	or it may be one of JasperReports's predefined colors listed here:
 * 		http://jasperreports.sourceforge.net/api/net/sf/jasperreports/engine/type/ColorEnum.html
 *   	black, blue, cyan, darkGray, gray, green, lightGray, magenta, orange, pink, red, white, yellow 
 */
public class PieLabels implements JRChartCustomizer {

	private static Log log = LogFactory.getLog(PieLabels.class);

	/**
	 * 
	 */
	public PieLabels() {
		// TODO Auto-generated constructor stub
	}

	public void customize(JFreeChart chart, JRChart jasperChart) {
		log.debug("################## DEBUG info from PieLabels ##################");
		String labelFontName = null;
		int    labelFontStyle = -1;
		int    labelFontSize = -1;
		double interiorGap = -1.0;
		double maximumLabelWidth= -1.0;
		boolean useLabels = true;
		boolean innerLabel = false;
		String predefinedColors = null;

		// Gather all of the properties set on the chart object
		// Font default information:
		//   If the font name is null then this chart customizer makes no change to the default font choice.
		//   If the font name is not valid, then the font system will map the Font instance to "Dialog"
		//   If unspecified, the font style will be PLAIN
		//   If unspecified, the font size will be 8 (we assume these labels should generally be quite small)
		JRPropertiesMap pm = jasperChart.getPropertiesMap();
		if (pm != null) {
			labelFontName     = pm.getProperty("LabelFontName");
			labelFontStyle    = (pm.getProperty("LabelFontStyle") == null) ? Font.PLAIN : Integer.parseInt(pm.getProperty("LabelFontStyle"));
			labelFontSize     = (pm.getProperty("LabelFontSize") == null) ? 8 : Integer.parseInt(pm.getProperty("LabelFontSize"));
			interiorGap       = (pm.getProperty("InteriorGap") == null) ? -1.0 : Double.parseDouble(pm.getProperty("InteriorGap"));
			maximumLabelWidth = (pm.getProperty("MaximumLabelWidth") == null) ? -1.0 : Double.parseDouble(pm.getProperty("MaximumLabelWidth"));
			predefinedColors  = pm.getProperty("PredefinedColors");
			useLabels = (pm.getProperty("UseLabels")==null)?true:Boolean.parseBoolean(pm.getProperty("UseLabels"));
			innerLabel = (pm.getProperty("InnerLabel")==null)?false:Boolean.parseBoolean(pm.getProperty("InnerLabel"));
		}
		log.debug(pm);
		log.debug("labelFontName: " + labelFontName);
		log.debug("labelFontStyle: " + labelFontStyle);
		log.debug("labelFontSize: " + labelFontSize);
		log.debug("interiorGap: " + interiorGap);
		log.debug("maximumLabelWidth: " + maximumLabelWidth);
		
		
		
		// This chart customizer requires that the PredefinedColors string is well formatted.
		// If it is not... we hope that things don't blow up. Hopefully we gracefully ignore badly formatted strings.
		// First split the string into an array of strings of the form "Pie Piece Key:Color"
		String[] entries = predefinedColors.split(";");
		Map<String, Color> pieSections = new HashMap<String, Color>();
		for (int i=0; i<entries.length; i++) {
			String value = entries[i];
			if (value != null) {
				// For each value we split it into its 2 constituent parts. The first is only required to be String, so there is no risk.
				// The second part is the color. We rely on JRColorUtil to deal with any badly defined colors.
				String[] pair = entries[i].split(":");
				if (pair[0] != null && pair[1] != null) {
					pieSections.put(pair[0], JRColorUtil.getColor(pair[1], null));
				}
			}
		}
		
		
		// This customizer works only with Pie Charts.
		// It silently ignores all other chart types.
		Plot plot = chart.getPlot();
		if (plot instanceof PiePlot) {
			PiePlot piePlot = (PiePlot)plot;
			Font labelFont = null;
			try {
				labelFont = new Font(labelFontName, labelFontStyle, labelFontSize);
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			if (labelFontName != null) {
				piePlot.setLabelFont(labelFont);
			}
			
			if (interiorGap != -1.0) {
				piePlot.setInteriorGap(interiorGap);
			}
			if (maximumLabelWidth != -1.0) {
				piePlot.setMaximumLabelWidth(maximumLabelWidth);
			}
			
			if (predefinedColors != null) {
				for (String key : pieSections.keySet()) {
					piePlot.setSectionPaint(key, pieSections.get(key));
				}
			}
			
			if(!useLabels) {
				piePlot.setLabelGenerator(null);
			}
			
			if(innerLabel){
				piePlot.setLegendLabelGenerator(new StandardPieSectionLabelGenerator("{2}"));
				piePlot.setSimpleLabels(Boolean.TRUE);
			}
		}	
	}
}