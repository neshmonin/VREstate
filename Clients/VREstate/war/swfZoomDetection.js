/**
 * @classdescription: 
 * @author: Sebastian Martens; sebastian@sebastian-martens.de: blog.sebastian-martens.de
 * @copyright: Creative Commons. Free to use "as is"
 * @svn: $Id: swfZoomDetection.js 10 2010-08-24 20:02:52Z dinnerout $
 * @uses: swfObject
 */ 
var swfZoomDetection = {
	
	_nodeId: 'swfZoomDetectionNode2',
	
	_detectSWFWidth:10, // width of test flash
	_detectSWFHeight:10, // height of test flash
	_flashVersion:"9.0.0", // minimum version of test flash
	
	_flashVars:{}, // flash vars
	_params:{ menu:false, wmode:'transparent', scale:'noScale' }, // flash parameters
	_attributes:{ id: this._nodeId, name: this._nodeId, style:'position:absolute;right:0px;bottom:0px' }, // flash atributes
	
	onZoomChange: null,
	getInitIfOne: false, // if true zoomChange handler will initial be triggered if zoomlevel is 1
	initCallOnly: false, // if true, flash node will be deleted after initial call
	
	currentScale: null,
	
	/**
	 * constructor 
	 */ 
	init:function( obj ){
		// mixes given parameter into this
		
		this.mixIn( obj );
		
		this._isInitialized = false;
		return this.createNode();
	},
	
	/**
	 * mix a given object into this object
	 * @param {Object} obj - given object with parameters
	 */	
	mixIn: function( obj, scope ){
		var item;
		if(!scope){ 
			scope = this;
		}
		for(item in obj){
			scope[ item ] = obj[item];
		}
	},
	
	/**
	 * load handler, called after document is fully loaded
	 * adds flash node via swfObjects into DOM
	 *  
	 * @param {Object} obj - event parameter
	 */
	createNode:function( obj ){
		var node = document.createElement('DIV'),bodyNode;
		node.setAttribute('id',this._nodeId);
		
		bodyNode = document.getElementsByTagName("BODY")[0]; 
		bodyNode.appendChild( node );
		
		this._zoomDetectSWF = node;	
		
		if( this.hasPlayerVersion() ){
			swfobject.embedSWF("swfZoomDetection.swf", this._nodeId, this._detectSWFWidth, this._detectSWFHeight, this._flashVersion,"expressInstall.swf", this._flashvars, this._params, this._attributes);	
		}else{
			return false;			
		}
		
		return true;
	},
	
	/**
	 * 
	 */
	hasPlayerVersion:function() {
		var pv = swfobject.getFlashPlayerVersion(), v = this._flashVersion.split(".");
		v[0] = parseInt(v[0], 10);
		v[1] = parseInt(v[1], 10) || 0;
		v[2] = parseInt(v[2], 10) || 0;
		return (pv['major'] > v[0] || (pv['major'] == v[0] && pv['minor'] > v[1]) || (pv['major'] == v[0] && pv['minor'] == v[1] && pv['release'] >= v[2])) ? true : false;
	},
	
	/**
	 * init function called from flash, passes init parameters to flash
	 * @param {Object} obj
	 */
	swfZoomDetectInit:function( obj ){
		var obj = {
			'setHeight':this._detectSWFHeight,
			'setWidth':this._detectSWFWidth,
			'isOnZoomChangeSet': (this.onZoomChange!=null)
		}
		
		return obj;
	},
	
	/**
	 * zoom change handler, called from flash when browser zoom changes
	 * @param {Object} obj - object from flash containing the scale value
	 */
	zoomChangeHandler:function( obj ){
		var node;
		
		this.currentScale = obj.scale;
		if(obj.scale!=1||!obj.init||this.getInitIfOne){ 
			this.onZoomChange(obj);
		}
		
		// remove node if initCall only
		if(obj.init&&this.initCallOnly){
			node = document.getElementById(this._nodeId);
			node.parentNode.removeChild( node );
		}
	},
	
	/**
	 * getter method for browser is in zoom method, true if browser is zoomed 
	 * @return Boolean, true if browser is zoomed, return null if not ready
	 */
	isBrowserZoom:function(){
		if( this.currentScale===null ){ 
			return null;
		}
		return (this.currentScale==1);
	},
	
	/**
	 * return the current scale mode
	 * @return Number the current scale value, null if not ready
	 */
	getZoomLevel: function(){
		return this.currentScale;
	}	
	
};