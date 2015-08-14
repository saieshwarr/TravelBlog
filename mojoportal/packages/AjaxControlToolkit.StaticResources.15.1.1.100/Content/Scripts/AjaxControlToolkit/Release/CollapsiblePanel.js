﻿Type.registerNamespace("Sys.Extended.UI");Sys.Extended.UI.CollapsiblePanelExpandDirection=function(){throw Error.invalidOperation();};Sys.Extended.UI.CollapsiblePanelExpandDirection.prototype={Horizontal:0,Vertical:1};Sys.Extended.UI.CollapsiblePanelExpandDirection.registerEnum("Sys.Extended.UI.CollapsiblePanelExpandDirection",!1);Sys.Extended.UI.CollapsiblePanelBehavior=function(n){Sys.Extended.UI.CollapsiblePanelBehavior.initializeBase(this,[n]);this._collapsedSize=0;this._expandedSize=0;this._scrollContents=null;this._collapsed=!1;this._expandControlID=null;this._collapseControlID=null;this._textLabelID=null;this._collapsedText=null;this._expandedText=null;this._imageControlID=null;this._expandedImage=null;this._collapsedImage=null;this._suppressPostBack=null;this._autoExpand=null;this._autoCollapse=null;this._expandDirection=Sys.Extended.UI.CollapsiblePanelExpandDirection.Vertical;this._collapseClickHandler=null;this._expandClickHandler=null;this._panelMouseEnterHandler=null;this._panelMouseLeaveHandler=null;this._childDiv=null;this._animation=null};Sys.Extended.UI.CollapsiblePanelBehavior.prototype={initialize:function(){var n,t,i,r,u;if(Sys.Extended.UI.CollapsiblePanelBehavior.callBaseMethod(this,"initialize"),n=this.get_element(),this._animation=new Sys.Extended.UI.Animation.LengthAnimation(n,.25,10,"style",null,0,0,"px"),this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Vertical?this._animation.set_propertyKey("height"):this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Horizontal&&this._animation.set_propertyKey("width"),this._animation.add_ended(Function.createDelegate(this,this._onAnimateComplete)),this._suppressPostBack==null&&(n.tagName=="INPUT"&&n.type=="checkbox"?(this._suppressPostBack=!1,this.raisePropertyChanged("SuppressPostBack")):n.tagName=="A"&&(this._suppressPostBack=!0,this.raisePropertyChanged("SuppressPostBack"))),t=Sys.Extended.UI.CollapsiblePanelBehavior.callBaseMethod(this,"get_ClientState"),t&&t!=""&&(i=Boolean.parse(t),this._collapsed!=i&&(this._collapsed=i,this.raisePropertyChanged("Collapsed"))),this._setupChildDiv(),this._collapsed?this._setTargetSize(this._getCollapsedSize()):this._setTargetSize(this._getExpandedSize()),this._setupState(this._collapsed),this._collapseControlID==this._expandControlID?(this._collapseClickHandler=Function.createDelegate(this,this.togglePanel),this._expandClickHandler=null):(this._collapseClickHandler=Function.createDelegate(this,this.collapsePanel),this._expandClickHandler=Function.createDelegate(this,this.expandPanel)),this._autoExpand&&(this._panelMouseEnterHandler=Function.createDelegate(this,this._onMouseEnter),$addHandler(n,"mouseover",this._panelMouseEnterHandler)),this._autoCollapse&&(this._panelMouseLeaveHandler=Function.createDelegate(this,this._onMouseLeave),$addHandler(n,"mouseout",this._panelMouseLeaveHandler)),this._collapseControlID)if(r=$get(this._collapseControlID),r)$addHandler(r,"click",this._collapseClickHandler);else throw Error.argument("CollapseControlID",String.format(Sys.Extended.UI.Resources.CollapsiblePanel_NoControlID,this._collapseControlID));if(this._expandControlID&&this._expandClickHandler)if(u=$get(this._expandControlID),u)$addHandler(u,"click",this._expandClickHandler);else throw Error.argument("ExpandControlID",String.format(Sys.Extended.UI.Resources.CollapsiblePanel_NoControlID,this._expandControlID));},dispose:function(){var i=this.get_element(),n,t;this._collapseClickHandler&&(n=this._collapseControlID?$get(this._collapseControlID):null,n&&$removeHandler(n,"click",this._collapseClickHandler),this._collapseClickHandler=null);this._expandClickHandler&&(t=this._expandControlID?$get(this._expandControlID):null,t&&$removeHandler(t,"click",this._expandClickHandler),this._expandClickHandler=null);this._panelMouseEnterHandler&&$removeHandler(i,"mouseover",this._panelMouseEnterHandler);this._panelMouseLeaveHandler&&$removeHandler(i,"mouseout",this._panelMouseLeaveHandler);this._animation&&(this._animation.dispose(),this._animation=null);Sys.Extended.UI.CollapsiblePanelBehavior.callBaseMethod(this,"dispose")},togglePanel:function(n){this._toggle(n)},expandPanel:function(n){this._doOpen(n)},collapsePanel:function(n){this._doClose(n)},_checkCollapseHide:function(){if(this._collapsed&&this._getTargetSize()==0){var n=this.get_element(),t=$common.getCurrentStyle(n,"display");return n.oldDisplay||t=="none"||(n.oldDisplay=t,n.style.display="none"),!0}return!1},_doClose:function(n){var t=new Sys.CancelEventArgs;if((this.raiseCollapsing(t),!t.get_cancel())&&(this._animation&&(this._animation.stop(),this._animation.set_startValue(this._getTargetSize()),this._animation.set_endValue(this._getCollapsedSize()),this._animation.play()),this._setupState(!0),this._suppressPostBack))if(n&&n.preventDefault)n.preventDefault();else return n&&(n.returnValue=!1),!1},_doOpen:function(n){var i=new Sys.CancelEventArgs,t;if((this.raiseExpanding(i),!i.get_cancel())&&(this._animation&&(this._animation.stop(),t=this.get_element(),this._checkCollapseHide()&&$common.getCurrentStyle(t,"display",t.style.display)&&(t.oldDisplay?t.style.display=t.oldDisplay:t.style.removeAttribute?t.style.removeAttribute("display"):t.style.removeProperty("display"),t.oldDisplay=null),this._animation.set_startValue(this._getTargetSize()),this._animation.set_endValue(this._getExpandedSize()),this._animation.play()),this._setupState(!1),this._suppressPostBack))if(n&&n.preventDefault)n.preventDefault();else return n&&(n.returnValue=!1),!1},_onAnimateComplete:function(){var n=this.get_element();this._collapsed||this._expandedSize?this._checkCollapseHide():this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Vertical?this._childDiv.offsetHeight<=n.offsetHeight?(n.style.height="auto",this.raisePropertyChanged("TargetHeight")):this._checkCollapseHide():this._childDiv.offsetWidth<=n.offsetWidth?(n.style.width="auto",this.raisePropertyChanged("TargetWidth")):this._checkCollapseHide();this._collapsed?(this.raiseCollapseComplete(),this.raiseCollapsed(Sys.EventArgs.Empty)):(this.raiseExpandComplete(),this.raiseExpanded(new Sys.EventArgs))},_onMouseEnter:function(n){this._autoExpand&&this.expandPanel(n)},_onMouseLeave:function(n){this._autoCollapse&&this.collapsePanel(n)},_getExpandedSize:function(){return this._expandedSize?this._expandedSize:this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Vertical?this._childDiv.offsetHeight:this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Horizontal?this._childDiv.offsetWidth:void 0},_getCollapsedSize:function(){return this._collapsedSize?this._collapsedSize:0},_getTargetSize:function(){var n;return this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Vertical?n=this.get_TargetHeight():this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Horizontal&&(n=this.get_TargetWidth()),n===undefined&&(n=0),n},_setTargetSize:function(n){var i=this._collapsed||this._expandedSize,t=this.get_element();this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Vertical?i||n<t.offsetHeight?this.set_TargetHeight(n):(t.style.height="auto",this.raisePropertyChanged("TargetHeight")):this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Horizontal&&(i||n<t.offsetWidth?this.set_TargetWidth(n):(t.style.width="auto",this.raisePropertyChanged("TargetWidth")));this._checkCollapseHide()},_setupChildDiv:function(){var t=this._getTargetSize(),n=this.get_element(),i;for(this._childDiv=n.cloneNode(!1),n.id="",this._childDiv.style.visibility="visible",this._childDiv.style.display="";n.hasChildNodes();)i=n.childNodes[0],i=n.removeChild(i),this._childDiv.appendChild(i);n.setAttribute("style","");n.className="";n.style.border="0px";n.style.margin="0px";n.style.padding="0px";this._scrollContents?(this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Vertical?(n.style.overflowY="scroll",this._childDiv.style.overflowY=""):(n.style.overflowX="scroll",this._childDiv.style.overflowX=""),(Sys.Browser.agent==Sys.Browser.Safari||Sys.Browser.agent==Sys.Browser.Opera)&&(n.style.overflow="scroll",this._childDiv.style.overflow="")):(this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Vertical?(n.style.overflowY="hidden",this._childDiv.style.overflowY=""):(n.style.overflowX="hidden",this._childDiv.style.overflowX=""),(Sys.Browser.Agent==Sys.Browser.Safari||Sys.Browser.Agent==Sys.Browser.Opera)&&(n.style.overflow="hidden",this._childDiv.style.overflow=""));this._childDiv.style.position="";t==this._collapsedSize&&(this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Vertical?this._childDiv.style.height="auto":this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Horizontal&&(this._childDiv.style.width="auto"));n.appendChild(this._childDiv);n.style.visibility="visible";n.style.display="";t=this._collapsed?this._getCollapsedSize():this._getExpandedSize();this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Vertical?(n.style.height=t+"px",n.style.height=this._expandedSize?this._expandedSize+"px":"auto",this._childDiv.style.height="auto"):this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Horizontal&&(n.style.width=t+"px",n.style.width=this._expandedSize?this._expandedSize+"px":"auto",this._childDiv.style.width="auto")},_setupState:function(n){var i,t;n?(this._textLabelID&&this._collapsedText&&(i=$get(this._textLabelID),i&&(i.innerHTML=this._collapsedText)),this._imageControlID&&this._collapsedImage&&(t=$get(this._imageControlID),t&&t.src&&(t.src=this._collapsedImage,(this._expandedText||this._collapsedText)&&(t.title=this._collapsedText)))):(this._textLabelID&&this._expandedText&&(i=$get(this._textLabelID),i&&(i.innerHTML=this._expandedText)),this._imageControlID&&this._expandedImage&&(t=$get(this._imageControlID),t&&t.src&&(t.src=this._expandedImage,(this._expandedText||this._collapsedText)&&(t.title=this._expandedText))));this._collapsed!=n&&(this._collapsed=n,this.raisePropertyChanged("Collapsed"));Sys.Extended.UI.CollapsiblePanelBehavior.callBaseMethod(this,"set_ClientState",[this._collapsed.toString()])},_toggle:function(n){return this.get_Collapsed()?this.expandPanel(n):this.collapsePanel(n)},add_collapsing:function(n){this.get_events().addHandler("collapsing",n)},remove_collapsing:function(n){this.get_events().removeHandler("collapsing",n)},raiseCollapsing:function(n){var t=this.get_events().getHandler("collapsing");t&&t(this,n)},add_collapsed:function(n){this.get_events().addHandler("collapsed",n)},remove_collapsed:function(n){this.get_events().removeHandler("collapsed",n)},raiseCollapsed:function(n){var t=this.get_events().getHandler("collapsed");t&&t(this,n)},add_collapseComplete:function(n){this.get_events().addHandler("collapseComplete",n)},remove_collapseComplete:function(n){this.get_events().removeHandler("collapseComplete",n)},raiseCollapseComplete:function(){var n=this.get_events().getHandler("collapseComplete");n&&n(this,Sys.EventArgs.Empty)},add_expanding:function(n){this.get_events().addHandler("expanding",n)},remove_expanding:function(n){this.get_events().removeHandler("expanding",n)},raiseExpanding:function(n){var t=this.get_events().getHandler("expanding");t&&t(this,n)},add_expanded:function(n){this.get_events().addHandler("expanded",n)},remove_expanded:function(n){this.get_events().removeHandler("expanded",n)},raiseExpanded:function(n){var t=this.get_events().getHandler("expanded");t&&t(this,n)},add_expandComplete:function(n){this.get_events().addHandler("expandComplete",n)},remove_expandComplete:function(n){this.get_events().removeHandler("expandComplete",n)},raiseExpandComplete:function(){var n=this.get_events().getHandler("expandComplete");n&&n(this,Sys.EventArgs.Empty)},get_TargetHeight:function(){return this.get_element().offsetHeight},set_TargetHeight:function(n){this.get_element().style.height=n+"px";this.raisePropertyChanged("TargetHeight")},get_TargetWidth:function(){return this.get_element().offsetWidth},set_TargetWidth:function(n){this.get_element().style.width=n+"px";this.raisePropertyChanged("TargetWidth")},get_Collapsed:function(){return this._collapsed},set_Collapsed:function(n){this.get_isInitialized()&&this.get_element()&&n!=this.get_Collapsed()?this.togglePanel():(this._collapsed=n,this.raisePropertyChanged("Collapsed"))},get_CollapsedSize:function(){return this._collapsedSize},set_CollapsedSize:function(n){this._collapsedSize!=n&&(this._collapsedSize=n,this.raisePropertyChanged("CollapsedSize"))},get_ExpandedSize:function(){return this._expandedSize},set_ExpandedSize:function(n){this._expandedSize!=n&&(this._expandedSize=n,this.raisePropertyChanged("ExpandedSize"))},get_CollapseControlID:function(){return this._collapseControlID},set_CollapseControlID:function(n){this._collapseControlID!=n&&(this._collapseControlID=n,this.raisePropertyChanged("CollapseControlID"))},get_ExpandControlID:function(){return this._expandControlID},set_ExpandControlID:function(n){this._expandControlID!=n&&(this._expandControlID=n,this.raisePropertyChanged("ExpandControlID"))},get_ScrollContents:function(){return this._scrollContents},set_ScrollContents:function(n){this._scrollContents!=n&&(this._scrollContents=n,this.raisePropertyChanged("ScrollContents"))},get_SuppressPostBack:function(){return this._suppressPostBack},set_SuppressPostBack:function(n){this._suppressPostBack!=n&&(this._suppressPostBack=n,this.raisePropertyChanged("SuppressPostBack"))},get_TextLabelID:function(){return this._textLabelID},set_TextLabelID:function(n){this._textLabelID!=n&&(this._textLabelID=n,this.raisePropertyChanged("TextLabelID"))},get_ExpandedText:function(){return this._expandedText},set_ExpandedText:function(n){this._expandedText!=n&&(this._expandedText=n,this.raisePropertyChanged("ExpandedText"))},get_CollapsedText:function(){return this._collapsedText},set_CollapsedText:function(n){this._collapsedText!=n&&(this._collapsedText=n,this.raisePropertyChanged("CollapsedText"))},get_ImageControlID:function(){return this._imageControlID},set_ImageControlID:function(n){this._imageControlID!=n&&(this._imageControlID=n,this.raisePropertyChanged("ImageControlID"))},get_ExpandedImage:function(){return this._expandedImage},set_ExpandedImage:function(n){this._expandedImage!=n&&(this._expandedImage=n,this.raisePropertyChanged("ExpandedImage"))},get_CollapsedImage:function(){return this._collapsedImage},set_CollapsedImage:function(n){this._collapsedImage!=n&&(this._collapsedImage=n,this.raisePropertyChanged("CollapsedImage"))},get_AutoExpand:function(){return this._autoExpand},set_AutoExpand:function(n){this._autoExpand!=n&&(this._autoExpand=n,this.raisePropertyChanged("AutoExpand"))},get_AutoCollapse:function(){return this._autoCollapse},set_AutoCollapse:function(n){this._autoCollapse!=n&&(this._autoCollapse=n,this.raisePropertyChanged("AutoCollapse"))},get_ExpandDirection:function(){return this._expandDirection==Sys.Extended.UI.CollapsiblePanelExpandDirection.Vertical},set_ExpandDirection:function(n){this._expandDirection!=n&&(this._expandDirection=n,this.raisePropertyChanged("ExpandDirection"))}};Sys.Extended.UI.CollapsiblePanelBehavior.registerClass("Sys.Extended.UI.CollapsiblePanelBehavior",Sys.Extended.UI.BehaviorBase);