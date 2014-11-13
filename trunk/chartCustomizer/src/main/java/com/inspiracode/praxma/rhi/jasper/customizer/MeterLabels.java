package com.inspiracode.praxma.rhi.jasper.customizer;

import java.awt.Font;

import net.sf.jasperreports.engine.JRChart;
import net.sf.jasperreports.engine.JRChartCustomizer;
import net.sf.jasperreports.engine.JRPropertiesMap;

import org.apache.commons.logging.Log;
import org.apache.commons.logging.LogFactory;
import org.jfree.chart.JFreeChart;
import org.jfree.chart.plot.MeterPlot;
import org.jfree.chart.plot.Plot;

/**
 * @author mdahlman
 * This chart customizer allows the report developer to set the following JFreeChart property
 * which is not otherwise set-able in iReport:
 * 
 *   Tick Label Font
 * 
 * This property applies only to Pie plots.
 * Charts that do not use pie plots will ignore these properties.
 * The properties should be set using the "Properties expressions" field in iReport. 
 * This results in .jrxml like the following:
 * 
 *   <property name="TickLabelFontName" value="Verdana"/>
 *   <property name="TickLabelFontStyle" value="Font.PLAIN"/>
 *   <property name="TickLabelFontSize" value="8"/>
 *
 */
public class MeterLabels implements JRChartCustomizer {

	private static Log log = LogFactory.getLog(MeterLabels.class);

	/**
	 * 
	 */
	public MeterLabels() {
		// TODO Auto-generated constructor stub
	}

	public void customize(JFreeChart chart, JRChart jasperChart) {
		log.debug("################## DEBUG info from MeterLabels ##################");

		// Gather all of the properties set on the chart object
		// Font default information:
		//   If the font name is null then this chart customizer makes no change to the default font choice.
		//   If the font name is not valid, then the font system will map the Font instance to "Dialog"
		//   If unspecified, the font style will be PLAIN
		//   If unspecified, the font size will be 8 (we assume these labels should generally be quite small)
		JRPropertiesMap pm = jasperChart.getPropertiesMap();
		String tickLabelFontName  = pm.getProperty("TickLabelFontName");
		int    tickLabelFontStyle = (pm.getProperty("TickLabelFontStyle") == null) ? Font.PLAIN : Integer.parseInt(pm.getProperty("TickLabelFontStyle"));
		int    tickLabelFontSize  = (pm.getProperty("TickLabelFontSize") == null) ? 8 : Integer.parseInt(pm.getProperty("TickLabelFontSize"));
		log.debug(pm);
		log.debug("tickLabelFontName: " + tickLabelFontName);
		log.debug("tickLabelFontStyle: " + tickLabelFontStyle);
		log.debug("tickLabelFontSize: " + tickLabelFontSize);

		// This customizer works only with Meter Charts.
		// It silently ignores all other chart types.
		Plot plot = chart.getPlot();
		if (plot instanceof MeterPlot) {
			MeterPlot meterPlot = (MeterPlot)plot;
			Font labelFont = null;
			try {
				labelFont = new Font(tickLabelFontName, tickLabelFontStyle, tickLabelFontSize);
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			if (labelFont != null) {
				meterPlot.setTickLabelFont(labelFont);
			}
		}	
	}
}
