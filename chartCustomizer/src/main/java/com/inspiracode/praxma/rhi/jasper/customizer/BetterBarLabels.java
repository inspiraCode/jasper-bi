package com.inspiracode.praxma.rhi.jasper.customizer;

import net.sf.jasperreports.engine.JRAbstractChartCustomizer;
import net.sf.jasperreports.engine.JRChart;
import net.sf.jasperreports.engine.JRChartCustomizer;
import net.sf.jasperreports.engine.JRPropertiesMap;

import org.apache.commons.logging.Log;
import org.apache.commons.logging.LogFactory;
import org.jfree.chart.JFreeChart;
import org.jfree.chart.axis.CategoryAxis;
import org.jfree.chart.axis.NumberAxis;
import org.jfree.chart.axis.ValueAxis;
import org.jfree.chart.plot.*;
import org.jfree.chart.renderer.category.BarRenderer;
/**
 * @author mdahlman
 * This chart customizer allows the report developer to set the following JFreeChart properties
 * which are not otherwise set-able in iReport:
 * 
 *   UpperMargin
 *   MaximumCategoryLabelWidthRatio
 *   MaximumCategoryLabelLines
 *   ItemMargin
 *   IntegerTickUnits
 * 
 * These properties apply only to Category plots (e.g. line charts and bar charts).
 * Charts that are not category plots will ignore these properties.
 * The properties should be set using the "Properties expressions" field in iReport. 
 * This results in .jrxml like the following:
 * 
 *   <property name="MaximumCategoryLabelWidthRatio" value="1.5f"/>
 *   <property name="ItemMargin" value="0.0f"/>
 *   <property name="MaximumCategoryLabelLines" value="2"/>
 *   <property name="UpperMargin" value="0.40"/>
 *
 */
public class BetterBarLabels extends JRAbstractChartCustomizer implements
		JRChartCustomizer {

	/* (non-Javadoc)
	 * @see net.sf.jasperreports.engine.JRChartCustomizer#customize(org.jfree.chart.JFreeChart, net.sf.jasperreports.engine.JRChart)
	 */
	private static Log log = LogFactory.getLog(BetterBarLabels.class);

	public void customize(JFreeChart chart, JRChart jasperChart) {
		log.debug("################## DEBUG info from BetterBarLabels ##################");

		// Gather all of the properties set on the chart object
		JRPropertiesMap pm = jasperChart.getPropertiesMap();
		double upperMargin = (pm.getProperty("UpperMargin") == null) ? -1 : Double.parseDouble(pm.getProperty("UpperMargin"));
		float maximumCategoryLabelWidthRatio = (pm.getProperty("MaximumCategoryLabelWidthRatio") == null) ? -1 : Float.parseFloat(pm.getProperty("MaximumCategoryLabelWidthRatio"));
		float itemMargin = (pm.getProperty("ItemMargin") == null) ? -1 : Float.parseFloat(pm.getProperty("ItemMargin"));
		int maximumCategoryLabelLines = (pm.getProperty("MaximumCategoryLabelLines") == null) ? -1 : Integer.parseInt(pm.getProperty("MaximumCategoryLabelLines"));
		boolean useIntegerTickUnits = (pm.getProperty("IntegerTickUnits") == null || !pm.getProperty("IntegerTickUnits").equals("true")) ? false : true;
		log.debug(pm);
		System.out.println(pm);
				
		// This customizer works only with Category Plots (like Line Charts and Bar Charts).
		// It silently ignores all other chart types.
		Plot plot = chart.getPlot();
		if (plot instanceof CategoryPlot) {
			CategoryPlot categoryPlot = (CategoryPlot)plot;
			ValueAxis    valueAxis    = categoryPlot.getRangeAxis();
			CategoryAxis categoryAxis = categoryPlot.getDomainAxis();
			
			if (useIntegerTickUnits) {
				valueAxis.setStandardTickUnits(NumberAxis.createIntegerTickUnits());
			}
			
			// The default upper margin is 5%.
			// This is nearly always no good if labels are displayed.
			// We should calculate the height needed for the top label
			// and then set the upper margin appropriately.
			// Instead of doing that, we let the report designer choose a value.
			// The value must be a percentage between 0 and 1.
			if ( upperMargin >= 0 && upperMargin <= 1 ) {
				valueAxis.setUpperMargin(upperMargin);
			}
			
			// I don't know what the default MaximumCategoryLabelWidthRatio is,
			// but it's too small in many cases.
			if ( maximumCategoryLabelWidthRatio > 0 ) {
				categoryAxis.setMaximumCategoryLabelWidthRatio(maximumCategoryLabelWidthRatio);
			}
			
			// The ItemMargin is the space between bars within a single category.
			// The default value is 10% (0.10).
			// It's common to want this smaller.
			if (categoryPlot.getRenderer() instanceof BarRenderer) {
				BarRenderer barRenderer = (BarRenderer)categoryPlot.getRenderer();
				if (itemMargin >= 0 && itemMargin <= 1) {
					barRenderer.setItemMargin(itemMargin);
				} 
			}
			
			
			// By default category labels are a single line.
			if ( maximumCategoryLabelLines > 0 ) {
				categoryAxis.setMaximumCategoryLabelLines(maximumCategoryLabelLines);
			}
			
		}

	}

}
