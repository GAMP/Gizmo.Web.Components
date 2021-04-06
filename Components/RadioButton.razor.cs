using Microsoft.AspNetCore.Components;

namespace Gizmo.Web.Components
{
    public partial class RadioButton : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public RadioButton()
        {
            ClassMapper.Add("radio")
                .If("disabled", () => IsDisabled);
        } 
        #endregion

        /// <summary>
        /// Gets or sets if element is disabled.
        /// </summary>
        [Parameter()]
        public bool IsDisabled
        {
            get;set;
        }

        /// <summary>
        /// Gets or sets if element is checked.
        /// </summary>
        [Parameter()]
        public bool IsChecked
        {
            get;set;
        }

        /// <summary>
        /// Gets or sets radio button name.
        /// </summary>
        [Parameter()]
        public string GroupName
        {
            get;set;
        }
    }
}
