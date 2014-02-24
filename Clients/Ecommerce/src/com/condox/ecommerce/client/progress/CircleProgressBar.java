package com.condox.ecommerce.client.progress;

import com.google.gwt.core.client.GWT;
import com.google.gwt.dom.client.Style;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.FlowPanel;
 
/**
* Круговой прогресс бар для GWT
* @author kos1nus
*
*/
public class CircleProgressBar extends Composite
{
 
private static CircleProgressBarUiBinder uiBinder = GWT.create(CircleProgressBarUiBinder.class);
interface CircleProgressBarUiBinder extends	UiBinder<Widget, CircleProgressBar> {}
@UiField FlowPanel volume_knob;
@UiField FlowPanel volume;
@UiField FlowPanel progress_bar;
@UiField FlowPanel l_bar;
@UiField FlowPanel l_bar_fon;
@UiField FlowPanel r_bar;
@UiField FlowPanel r_bar_fon;
/**
* цвет линии прогресс бара
*/
String color;
/**
* цвет фона
*/
String colorFon;
 
/**
*
* @param size1 (<code>Integer</code>) развем круга
* @param size2 (<code>Integer</code>) толщина линии прогресс бара
* @param color (<code>String</code>) цвет линии прогресс бара
* @param colorFon (<code>String</code>) цвет фона
*/
public CircleProgressBar(int size1, int size2, String color, String colorFon)
{	
initWidget(uiBinder.createAndBindUi(this));
/**
* размер центрального круга
*/
int size_volume_knob = size1 - size2*2;
/**
* размер кругов, отвечающих за прогресс бар. Размер должен быть меньше, чем размер кругов фона, иначе будут артефакты оставаться
*/
int size_rl_bar = size1-2;
this.color = color;
this.colorFon = colorFon;
/**
* размер прогресс бара
*/
volume.getElement().getStyle().setWidth(size1, Style.Unit.PX);
volume.getElement().getStyle().setHeight(size1, Style.Unit.PX);
/**
* центральный круг
*/
if(size2==size1 || size2/2>=size1){
volume_knob.getElement().getStyle().setDisplay(Style.Display.NONE);
}else{
volume_knob.getElement().getStyle().setWidth(size_volume_knob, Style.Unit.PX);
volume_knob.getElement().getStyle().setHeight(size_volume_knob, Style.Unit.PX);
volume_knob.getElement().getStyle().setTop(size2, Style.Unit.PX);
volume_knob.getElement().getStyle().setLeft(size2, Style.Unit.PX);
volume_knob.getElement().getStyle().setPropertyPx("borderRadius", size_volume_knob);
volume_knob.getElement().getStyle().setBackgroundColor(colorFon);	
}
 
/**
*
*/
l_bar_fon.getElement().getStyle().setWidth(size1, Style.Unit.PX);
l_bar_fon.getElement().getStyle().setHeight(size1, Style.Unit.PX);
l_bar_fon.getElement().getStyle().setPropertyPx("borderRadius", size1);;
l_bar_fon.getElement().getStyle().setProperty("clip", "rect(0px, "+size1/2+"px, "+size1+"px, 0px)");
l_bar_fon.getElement().getStyle().setBackgroundColor(colorFon);
/**
*
*/
l_bar.getElement().getStyle().setWidth(size_rl_bar, Style.Unit.PX);
l_bar.getElement().getStyle().setHeight(size_rl_bar, Style.Unit.PX);
l_bar.getElement().getStyle().setPropertyPx("borderRadius", size_rl_bar);
l_bar.getElement().getStyle().setProperty("clip", "rect(0px, "+size_rl_bar/2+"px, "+size_rl_bar+"px, 0px)");
l_bar.getElement().getStyle().setBackgroundColor(color);
/**
*
*/
r_bar_fon.getElement().getStyle().setWidth(size1, Style.Unit.PX);
r_bar_fon.getElement().getStyle().setHeight(size1, Style.Unit.PX);
r_bar_fon.getElement().getStyle().setPropertyPx("borderRadius", size1);
r_bar_fon.getElement().getStyle().setProperty("clip", "rect(0px, "+size1+"px, "+size1+"px, "+size1/2+"px)");
r_bar_fon.getElement().getStyle().setBackgroundColor(colorFon);	
/**
*
*/
r_bar.getElement().getStyle().setWidth(size_rl_bar, Style.Unit.PX);
r_bar.getElement().getStyle().setHeight(size_rl_bar, Style.Unit.PX);
r_bar.getElement().getStyle().setPropertyPx("borderRadius", size_rl_bar);
r_bar.getElement().getStyle().setProperty("clip", "rect(0px, "+size_rl_bar+"px, "+size_rl_bar+"px, "+size_rl_bar/2+"px)");
r_bar.getElement().getStyle().setBackgroundColor(color);
 
}
/**
* Задаем состояние в процентах
* @param percent (<code>Integer</code>) проценты
*/
public void setStatePercent(int percent){
if(percent>100) percent = 100;
if(percent<0) percent = 0;
int degrees = (int) (percent*3.6);
setStateDegrees(degrees);
}	
/**
* Задаем состояние в градусах
* @param degrees (<code>Integer</code>) градусы
*/
public void setStateDegrees(int degrees){
if(degrees>360) degrees = 360;
if(degrees<0) degrees = 0;
if(degrees>=180){
degrees = 360-degrees;
l_bar.getElement().getStyle().setBackgroundColor(color);
}else{
degrees = 180-degrees;
l_bar.getElement().getStyle().setBackgroundColor(colorFon);	
}
l_bar.getElement().getStyle().setProperty("WebkitTransform", "rotate(-"+degrees+"deg)");
l_bar.getElement().getStyle().setProperty("MozTransform", "rotate(-"+degrees+"deg)");
l_bar.getElement().getStyle().setProperty("MsTransform", "rotate(-"+degrees+"deg)");
l_bar.getElement().getStyle().setProperty("OTransform", "rotate(-"+degrees+"deg)");
l_bar.getElement().getStyle().setProperty("Transform", "rotate(-"+degrees+"deg)");
l_bar_fon.getElement().getStyle().setProperty("WebkitTransform", "rotate(-"+degrees+"deg)");
l_bar_fon.getElement().getStyle().setProperty("MozTransform", "rotate(-"+degrees+"deg)");
l_bar_fon.getElement().getStyle().setProperty("MsTransform", "rotate(-"+degrees+"deg)");
l_bar_fon.getElement().getStyle().setProperty("OTransform", "rotate(-"+degrees+"deg)");
l_bar_fon.getElement().getStyle().setProperty("Transform", "rotate(-"+degrees+"deg)");
r_bar_fon.getElement().getStyle().setProperty("WebkitTransform", "rotate(-"+degrees+"deg)");
r_bar_fon.getElement().getStyle().setProperty("MozTransform", "rotate(-"+degrees+"deg)");
r_bar_fon.getElement().getStyle().setProperty("MsTransform", "rotate(-"+degrees+"deg)");
r_bar_fon.getElement().getStyle().setProperty("OTransform", "rotate(-"+degrees+"deg)");
r_bar_fon.getElement().getStyle().setProperty("Transform", "rotate(-"+degrees+"deg)");
}
}