
/**
*  Ajax Autocomplete for jQuery, version 1.2.7
*  (c) 2013 Tomas Kirda
*
*  Ajax Autocomplete for jQuery is freely distributable under the terms of an MIT-style license.
*  For details, see the web site: http://www.devbridge.com/projects/autocomplete/jquery/
*
*/

/*jslint  browser: true, white: true, plusplus: true */
/*global define, window, document, jQuery */

// Expose plugin as an AMD module if AMD loader is present:
(function(factory) {
    'use strict';
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['jquery'], factory);
    } else {
        // Browser globals
        factory(jQuery);
    }
} (function(j) {
    'use strict';

    var 
        utils = (function() {
            return {
                escapeRegExChars: function(value) {
                    return value.replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, "\\$&");
                },
                createNode: function(html) {
                    var div = document.createElement('div');
                    div.innerHTML = html;
                    return div.firstChild;
                }
            };
        } ()),

        keys = {
            ESC: 27,
            TAB: 9,
            RETURN: 13,
            LEFT: 37,
            UP: 38,
            RIGHT: 39,
            DOWN: 40
        };

    function Autocomplete(el, options) {
        var noop = function() { },
            that = this,
            defaults = {
                autoSelectFirst: false,
                appendTo: 'body',
                serviceUrl: null,
                methodName: null,
                lookup: null,
                width: 'auto',
                minChars: 1,
                maxHeight: 300,
                deferRequestBy: 0,
                //                params: {},
                formatResult: Autocomplete.formatResult,
                delimiter: null,
                zIndex: 9999,
                //                type: 'GET',
                noCache: false,
                //                onSearchStart: noop,
                //                onSearchComplete: noop,
                containerClass: 'autocomplete-suggestions',
                tabDisabled: false,
                //                dataType: 'text',
                //                currentRequest: null,
                lookupFilter: function(suggestion, originalQuery, queryLowerCase) {
                    return suggestion.value.toLowerCase().indexOf(queryLowerCase) !== -1;
                },
                //                paramName: 'query',
                //                transformResult: function (response) {
                //                    return typeof response === 'string' ? j.parseJSON(response) : response;
                //                }
                OnSelect: null,           //当选中一条记录时（包括输入完成时）触发   functio(suggestion.value, suggestion.data )
                OnFullInputError: null,   //如果输入不能完全匹配，onblur时触发此事件 function(sender)
                OnError: null              //系统回调出错时触发 function(retMessage)
            };

        // Shared variables:
        that.element = el;
        that.el = j(el);
        that.suggestions = [];
        that.fullmatched = null;
        that.badQueries = [];
        that.selectedIndex = -1;
        that.currentValue = that.element.value;
        that.intervalId = 0;
        that.cachedResponse = [];
        that.onChangeInterval = null;
        that.onChange = null;
        that.isLocal = false;
        that.suggestionsContainer = null;
        that.options = j.extend({}, defaults, options);
        that.classes = {
            selected: 'autocomplete-selected',
            suggestion: 'autocomplete-suggestion'
        };
        that.hint = null;
        that.hintValue = '';
        that.selection = null;

        // Initialize and set options:
        that.initialize();
        that.setOptions(options);

        that._Webmethod = null;
        var serviceUrl = options.serviceUrl,
                methodName = options.methodName;
        if (serviceUrl != null) {
            that._Webmethod = new WebMethodProxy(serviceUrl, methodName);
            if (options.tag != null)
                that._Webmethod.AddParam("tag", options.tag);
        }
    }

    Autocomplete.utils = utils;

    j.Autocomplete = Autocomplete;

    Autocomplete.formatResult = function(suggestion, currentValue) {
        var pattern = '(' + utils.escapeRegExChars(currentValue) + ')';

        return suggestion.value.replace(new RegExp(pattern, 'gi'), '<strong>$1<\/strong>');
    };

    Autocomplete.prototype = {

        killerFn: null,

        initialize: function() {
            var that = this,
                suggestionSelector = '.' + that.classes.suggestion,
                selected = that.classes.selected,
                options = that.options,
            container;

            // Remove autocomplete attribute to prevent native suggestions:
            that.element.setAttribute('autocomplete', 'off');

            that.killerFn = function(e) {
                if (j(e.target).closest('.' + that.options.containerClass).length === 0) {
                    that.killSuggestions();
                    that.disableKillerFn();
                }
            };

            that.suggestionsContainer = Autocomplete.utils.createNode('<div class="' + options.containerClass + '" style="position: absolute; display: none;"></div>');

            container = j(that.suggestionsContainer);

            container.appendTo(options.appendTo);

            // Only set width if it was provided:
            if (options.width !== 'auto') {
                container.width(options.width);
            }

            // Listen for mouse over event on suggestions list:
            container.on('mouseover.autocomplete', suggestionSelector, function() {
                that.activate(j(this).data('index'));
            });

            // Deselect active element when mouse leaves suggestions container:
            container.on('mouseout.autocomplete', function() {
                that.selectedIndex = -1;
                container.children('.' + selected).removeClass(selected);
            });

            // Listen for click event on suggestions list:
            container.on('click.autocomplete', suggestionSelector, function() {
                that.select(j(this).data('index'));
            });

            that.fixPosition();

            that.fixPositionCapture = function() {
                if (that.visible) {
                    that.fixPosition();
                }
            };

            j(window).on('resize', that.fixPositionCapture);

            that.el.on('keydown.autocomplete', function(e) { that.onKeyPress(e); });
            that.el.on('keyup.autocomplete', function(e) { that.onKeyUp(e); });
            that.el.on('blur.autocomplete', function() { that.onBlur(); });
            that.el.on('focus.autocomplete', function() { that.fixPosition(); });
            that.el.on('change.autocomplete', function(e) { that.onKeyUp(e); });
        },

        onBlur: function() {
            var that = this,
                value = that.el.val().toLowerCase(),
                OnFullInputError = that.options.OnFullInputError;
            if (OnFullInputError && value != "" && that.fullmatched == null) {
                OnFullInputError(that.element);
            }
            this.enableKillerFn();
        },

        setOptions: function(suppliedOptions) {
            var that = this,
                options = that.options;

            j.extend(options, suppliedOptions);

            that.isLocal = j.isArray(options.lookup);

            if (that.isLocal) {
                options.lookup = that.verifySuggestionsFormat(options.lookup);
            }

            // Adjust height, width and z-index:
            j(that.suggestionsContainer).css({
                'max-height': options.maxHeight + 'px',
                'width': options.width + 'px',
                'z-index': options.zIndex
            });
        },

        clearCache: function() {
            this.cachedResponse = [];
            this.badQueries = [];
        },

        clear: function() {
            this.clearCache();
            this.currentValue = '';
            this.suggestions = [];
        },

        disable: function() {
            this.disabled = true;
        },

        enable: function() {
            this.disabled = false;
        },

        fixPosition: function() {
            var that = this,
                offset;

            // Don't adjsut position if custom container has been specified:
            if (that.options.appendTo !== 'body') {
                return;
            }

            offset = that.el.offset();

            j(that.suggestionsContainer).css({
                top: (offset.top + that.el.outerHeight()) + 'px',
                left: offset.left + 'px'
            });
        },

        enableKillerFn: function() {
            var that = this;
            j(document).on('click.autocomplete', that.killerFn);
        },

        disableKillerFn: function() {
            var that = this;
            j(document).off('click.autocomplete', that.killerFn);
        },

        killSuggestions: function() {
            var that = this;
            that.stopKillSuggestions();
            that.intervalId = window.setInterval(function() {
                that.hide();
                that.stopKillSuggestions();
            }, 300);
        },

        stopKillSuggestions: function() {
            window.clearInterval(this.intervalId);
        },

        isCursorAtEnd: function() {
            var that = this,
                valLength = that.el.val().length,
                selectionStart = that.element.selectionStart,
                range;

            if (typeof selectionStart === 'number') {
                return selectionStart === valLength;
            }
            if (document.selection) {
                range = document.selection.createRange();
                range.moveStart('character', -valLength);
                return valLength === range.text.length;
            }
            return true;
        },

        onKeyPress: function(e) {
            var that = this;

            // If suggestions are hidden and user presses arrow down, display suggestions:
            if (!that.disabled && !that.visible && e.which === keys.DOWN && that.currentValue) {
                that.suggest();
                return;
            }

            if (that.disabled || !that.visible) {
                return;
            }

            switch (e.which) {
                case keys.ESC:
                    that.el.val(that.currentValue);
                    that.hide();
                    break;
                case keys.RIGHT:
                    if (that.hint && that.options.onHint && that.isCursorAtEnd()) {
                        that.selectHint();
                        break;
                    }
                    return;
                case keys.TAB:
                    if (that.hint && that.options.onHint) {
                        that.selectHint();
                        return;
                    }
                    // Fall through to RETURN
                case keys.RETURN:
                    if (that.selectedIndex === -1) {
                        that.hide();
                        return;
                    }
                    that.select(that.selectedIndex);
                    if (e.which === keys.TAB && that.options.tabDisabled === false) {
                        return;
                    }
                    break;
                case keys.UP:
                    that.moveUp();
                    break;
                case keys.DOWN:
                    that.moveDown();
                    break;
                default:
                    return;
            }

            // Cancel event if function did not return:
            e.stopImmediatePropagation();
            e.preventDefault();
        },

        onKeyUp: function(e) {
            var that = this;

            if (that.disabled) {
                return;
            }

            switch (e.which) {
                case keys.UP:
                case keys.DOWN:
                    return;
            }

            clearInterval(that.onChangeInterval);

            if (that.currentValue !== that.el.val()) {
                that.findBestHint();
                if (that.options.deferRequestBy > 0) {
                    // Defer lookup in case when value changes very quickly:
                    that.onChangeInterval = setInterval(function() {
                        that.onValueChange();
                    }, that.options.deferRequestBy);
                } else {
                    that.onValueChange();
                }
            }
        },

        onValueChange: function() {
            var that = this,
                q;

            if (that.selection) {
                that.selection = null;
                (that.options.onInvalidateSelection || j.noop)();
            }

            clearInterval(that.onChangeInterval);
            that.currentValue = that.el.val();

            q = that.getQuery(that.currentValue);
            that.selectedIndex = -1;

            if (q.length < that.options.minChars) {
                that.hide();
            } else {
                that.getSuggestions(q);
            }
        },

        getQuery: function(value) {
            var delimiter = this.options.delimiter,
                parts;

            if (!delimiter) {
                return j.trim(value);
            }
            parts = value.split(delimiter);
            return j.trim(parts[parts.length - 1]);
        },

        getSuggestionsLocal: function(query) {
            var that = this,
                queryLowerCase = query.toLowerCase(),
                filter = that.options.lookupFilter;

            return {
                suggestions: j.grep(that.options.lookup, function(suggestion) {
                    return filter(suggestion, query, queryLowerCase);
                })
            };
        },

        getSuggestions: function(q) {
            var response,
                that = this;
            //                options = that.options,
            //                serviceUrl = options.serviceUrl;

            response = that.isLocal ? that.getSuggestionsLocal(q) : that.cachedResponse[q];

            if (response && j.isArray(response.suggestions)) {
                that.suggestions = response.suggestions;
                that.suggest();
            } else if (!that.isBadQuery(q)) {
                //options.params[options.paramName] = q;
                //if (options.onSearchStart.call(that.element, options.params) === false) {
                //    return;
                //}
                //if (j.isFunction(options.serviceUrl)) {
                //    serviceUrl = options.serviceUrl.call(that.element, q);
                //}
                //if(this.currentRequest != null) {
                //    this.currentRequest.abort();
                //}
                //this.currentRequest = j.ajax({
                //    url: serviceUrl,
                //    data: options.ignoreParams ? null : options.params,
                //    type: options.type,
                //    dataType: options.dataType
                //}).done(function (data) {
                //    that.processResponse(data, q);
                //    options.onSearchComplete.call(that.element, q);
                //});

                that._Webmethod.AddParam("strparam", q);
                that._Webmethod.Call(that.processResponse, that.onError, that);
            }
        },

        isBadQuery: function(q) {
            var badQueries = this.badQueries,
                i = badQueries.length;

            while (i--) {
                if (q.indexOf(badQueries[i]) === 0) {
                    return true;
                }
            }

            return false;
        },

        hide: function() {
            var that = this;
            that.visible = false;
            that.selectedIndex = -1;
            j(that.suggestionsContainer).hide();
            that.signalHint(null);
        },

        suggest: function() {
            this.fullmatched = null;
            if (this.suggestions.length === 0) {
                this.hide();
                //return;
            } else {

                var that = this,
                formatResult = that.options.formatResult,
                value = that.getQuery(that.currentValue),
                className = that.classes.suggestion,
                classSelected = that.classes.selected,
                container = j(that.suggestionsContainer),
                html = '',
                width;

                // Build suggestions inner HTML:
                j.each(that.suggestions, function(i, suggestion) {
                    html += '<div class="' + className + '" data-index="' + i + '">' + formatResult(suggestion, value) + '</div>';
                });

                // If width is auto, adjust width before displaying suggestions,
                // because if instance was created before input had width, it will be zero.
                // Also it adjusts if input width has changed.
                // -2px to account for suggestions border.
                if (that.options.width === 'auto') {
                    width = that.el.outerWidth() - 2;
                    container.width(width > 0 ? width : 300);
                }

                container.html(html).show();
                that.visible = true;

                // Select first value by default:
                if (that.options.autoSelectFirst) {
                    that.selectedIndex = 0;
                    container.children().first().addClass(classSelected);
                }

                that.fullmatched = that.findBestHint();
            }
            this.onFullSelect();
        },

        findBestHint: function() {
            var that = this,
                value = that.el.val().toLowerCase(),
                bestMatch = null,
                fullMatch = null;

            if (!value) {
                return fullMatch;
            }

            j.each(that.suggestions, function(i, suggestion) {
                var foundMatch = suggestion.value.toLowerCase().indexOf(value) === 0;
                if (foundMatch) {
                    bestMatch = suggestion;
                    if (suggestion.value.toLowerCase() == value)
                        fullMatch = suggestion;
                }
                //return !foundMatch;
            });

            that.signalHint(bestMatch);
            return fullMatch;
        },

        signalHint: function(suggestion) {
            var hintValue = '',
                that = this;
            if (suggestion) {
                hintValue = that.currentValue + suggestion.value.substr(that.currentValue.length);
            }
            if (that.hintValue !== hintValue) {
                that.hintValue = hintValue;
                that.hint = suggestion;
                (this.options.onHint || j.noop)(hintValue);
            }
        },

        verifySuggestionsFormat: function(suggestions) {
            // If suggestions is string array, convert them to supported format:
            if (suggestions.length && typeof suggestions[0] === 'string') {
                return j.map(suggestions, function(value) {
                    return { value: value, data: null };
                });
            }

            return suggestions;
        },

        //processResponse: function(response, originalQuery) {
        processResponse: function(response, that) {
            // var that = this,
            //         options = that.options,
            //         result = options.transformResult(response, originalQuery);
            // result.suggestions = that.verifySuggestionsFormat(result.suggestions);

            var options = that.options;
            var result = { suggestions: [] };

            var xmlDoc = CreateXmlFromString(response);
            var rootNode = xmlDoc.selectSingleNode("xml");
            if (rootNode == null) { that.onError("返回数据格式有误（不包含根结点xml）", that); return; }

            var mCode = GetAttributeValue(rootNode, "code");
            var mMsg = GetAttributeValue(rootNode, "msg");
            var mValue = GetAttributeValue(rootNode, "value");
            if (mCode != "0") { that.onError("webmethod return error(code=" + mCode + ") error msg: " + mMsg, that);return; }

            for (var i = 0; i < rootNode.childNodes.length; i++) {
                var itemNode = rootNode.childNodes[i];
                var itemValue = GetAttributeValue(itemNode, "value");
                var itemData = GetAttributeValue(itemNode, "data");
                result.suggestions.push({ value: itemValue, data: itemData });
            }

            // Cache results if cache is not disabled:
            if (!options.noCache) {
                that.cachedResponse[mValue] = result;
                if (result.suggestions.length === 0) {
                    that.badQueries.push(mValue);
                }
            }

            // Display suggestions only if returned query matches current value:
            //if (originalQuery === that.getQuery(that.currentValue)) {
            if (mValue === that.getQuery(that.currentValue)) {
                that.suggestions = result.suggestions;
                that.suggest();
            }
        },

        activate: function(index) {
            var that = this,
                activeItem,
                selected = that.classes.selected,
                container = j(that.suggestionsContainer),
                children = container.children();

            container.children('.' + selected).removeClass(selected);

            that.selectedIndex = index;

            if (that.selectedIndex !== -1 && children.length > that.selectedIndex) {
                activeItem = children.get(that.selectedIndex);
                j(activeItem).addClass(selected);
                return activeItem;
            }

            return null;
        },

        selectHint: function() {
            var that = this,
                i = j.inArray(that.hint, that.suggestions);

            that.select(i);
        },

        select: function(i) {
            var that = this;
            that.hide();
            that.OnSelect(i);
        },

        moveUp: function() {
            var that = this;

            if (that.selectedIndex === -1) {
                return;
            }

            if (that.selectedIndex === 0) {
                j(that.suggestionsContainer).children().first().removeClass(that.classes.selected);
                that.selectedIndex = -1;
                that.el.val(that.currentValue);
                that.findBestHint();
                return;
            }

            that.adjustScroll(that.selectedIndex - 1);
        },

        moveDown: function() {
            var that = this;

            if (that.selectedIndex === (that.suggestions.length - 1)) {
                return;
            }

            that.adjustScroll(that.selectedIndex + 1);
        },

        adjustScroll: function(index) {
            var that = this,
                activeItem = that.activate(index),
                offsetTop,
                upperBound,
                lowerBound,
                heightDelta = 25;

            if (!activeItem) {
                return;
            }

            offsetTop = activeItem.offsetTop;
            upperBound = j(that.suggestionsContainer).scrollTop();
            lowerBound = upperBound + that.options.maxHeight - heightDelta;

            if (offsetTop < upperBound) {
                j(that.suggestionsContainer).scrollTop(offsetTop);
            } else if (offsetTop > lowerBound) {
                j(that.suggestionsContainer).scrollTop(offsetTop - that.options.maxHeight + heightDelta);
            }

            that.el.val(that.getValue(that.suggestions[index].value));
            that.signalHint(null);
        },

        OnSelect: function(index) {
            this.fullmatched = this.suggestions[index];
            this.onFullSelect();
        },

        onFullSelect: function() {
            var suggestion = this.fullmatched;
            //window.status = suggestion ? "matched" : "not matched";
            if (suggestion == null)
                return;

            var that = this,
                onSelectCallback = that.options.OnSelect;
            that.currentValue = that.getValue(suggestion.value);
            that.el.val(that.currentValue);
            that.signalHint(null);
            that.suggestions = [];
            that.selection = suggestion;

            if (j.isFunction(onSelectCallback)) {
                onSelectCallback.call(that.element, suggestion.value, suggestion.data);
            }
        },

        onError: function(retMessage, that) {
            if (that != null) {
                if (that.options.OnError != null)            //将出错信息传向外部调用者（如果需要的话）
                    that.options.OnError(retMessage);
            }
        },

        getValue: function(value) {
            var that = this,
                delimiter = that.options.delimiter,
                currentValue,
                parts;

            if (!delimiter) {
                return value;
            }

            currentValue = that.currentValue;
            parts = currentValue.split(delimiter);

            if (parts.length === 1) {
                return value;
            }

            return currentValue.substr(0, currentValue.length - parts[parts.length - 1].length) + value;
        },

        dispose: function() {
            var that = this;
            that.el.off('.autocomplete').removeData('autocomplete');
            that.disableKillerFn();
            j(window).off('resize', that.fixPositionCapture);
            j(that.suggestionsContainer).remove();
        }
    };

    // Create chainable jQuery plugin:
    j.fn.autocomplete = function(options, args) {
        var dataKey = 'autocomplete';
        // If function invoked without argument return
        // instance of the first matched element:
        if (arguments.length === 0) {
            return this.first().data(dataKey);
        }

        return this.each(function() {
            var inputElement = j(this),
                instance = inputElement.data(dataKey);

            if (typeof options === 'string') {
                if (instance && typeof instance[options] === 'function') {
                    instance[options](args);
                }
            } else {
                // If instance already exists, destroy it:
                if (instance && instance.dispose) {
                    instance.dispose();
                }
                instance = new Autocomplete(this, options);
                inputElement.data(dataKey, instance);
            }
        });
    };
}));

var $j = jQuery.noConflict(); 
var AutoComplete = function(url, methodname, txtid) {
    this.option = {
        serviceUrl: null,
        methodName: null,
        minChars: 1,
        delimiter: /(,|;)\s*/, // regex or character
        maxHeight: 300,
        width: 200,  //width: 'auto', //200
        zIndex: 999, //不能超过1000
        deferRequestBy: 0, //miliseconds
        params: { country: 'Yes' }, //aditional parameters
        noCache: false, //default is false, set to true to disable caching
        lookup: null, //['a', 'b', 'c', 'd', 'aa', 'bb', 'a', 'abcdefdasfd'], //local lookup values
        autoSelectFirst: false,
        appendTo: 'body',
        containerClass: 'autocomplete-suggestions',
        tabDisabled: false,
        // callback function:
        //formatResult: Autocomplete.formatResult,
        lookupFilter: function(suggestion, originalQuery, queryLowerCase) { return suggestion.value.toLowerCase().indexOf(queryLowerCase) !== -1; },
        OnSelect: null,
        OnFullInputError: null,
        OnError: null
    };
    
    this.option.serviceUrl = url;
    this.option.methodName = methodname;
    this._txtid = txtid;

    //----公开事件接口------------------------------------------------------------
    this.OnSelect = null;           //当选中一条记录时（包括输入完成时）触发   functio(suggestion.value, suggestion.data )
    this.OnFullInputError = null;   //如果输入不能完全匹配，onblur时触发此事件 function(sender)
    this.OnError = null;            //系统回调出错时触发 function(retMessage)
    //----------------------------------------------------------------------------

    //----公开方法----------------------------------------------------------------
    this.active = function() {
        this.option.OnSelect = this.OnSelect;
        this.option.OnFullInputError = this.OnFullInputError;
        this.option.OnError = this.OnError;
        $j("#" + this._txtid).autocomplete(this.option);
    }
    //----------------------------------------------------------------------------
};

