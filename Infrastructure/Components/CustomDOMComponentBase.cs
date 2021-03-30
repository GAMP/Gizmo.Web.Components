﻿using Gizmo.Web.Components.Infrastructure;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// Base implementation for custom DOM components.
    /// </summary>
    public abstract class CustomDOMComponentBase : CustomComponentBase
    {
        #region CONSTRUCTOR
        protected CustomDOMComponentBase()
        {
            ClassMapper
                .Get(() => Class);
            StyleMapper.Get(() => Style);
        } 
        #endregion

        #region PRIVATE FIELDS

        private ElementReference _ref;
        private ClassMapper _classMapper;
        private StyleMapper _styleMapper;

        #endregion

        #region PROPERTIES
        
        #region PUBLIC

        /// <summary>
        /// Gets or sets DOM element ref.
        /// </summary>
        public virtual ElementReference Ref
        {
            get { return _ref; }
            set
            {
                _ref = value;
                RefBack?.Set(value);
            }
        }

        /// <summary>
        /// Gets or sets DOM element id.
        /// </summary>
        [Parameter()]
        public string Id { get; set; } = ComponentIdGenerator.Generate();

        /// <summary>
        /// Gets or sets additional attributes that will be applied to created DOM element.
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Gets or sets one or more classes for DOM element.
        /// </summary>
        [Parameter()]
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets inline style for DOM element.
        /// </summary>
        [Parameter()]
        public string Style { get; set; }

        #endregion

        #region PROTECTED

        /// <summary>
        /// Gets class mapper.
        /// </summary>
        protected ClassMapper ClassMapper
        {
            get
            {
                if (_classMapper == null)
                    _classMapper = new ClassMapper();
                return _classMapper;
            }
        }

        /// <summary>
        /// Gets style mapper.
        /// </summary>
        protected StyleMapper StyleMapper
        {
            get
            {
                if (_styleMapper == null)
                    _styleMapper = new StyleMapper();
                return _styleMapper;
            }
        }

        #endregion

        #endregion
    }
}
