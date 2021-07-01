﻿using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class RadioButton : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public RadioButton()
        {
        }
        #endregion

        #region PROPERTIES

        [Parameter]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets if element is disabled.
        /// </summary>
        [Parameter()]
        public bool IsDisabled
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets if element is checked.
        /// </summary>
        [Parameter()]
        public bool IsChecked
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets radio button name.
        /// </summary>
        [Parameter()]
        public string GroupName
        {
            get; set;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-radio")
                 .If("disabled", () => IsDisabled)
                 .AsString();

        #endregion

    }
}