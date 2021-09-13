using Microsoft.AspNetCore.Components;
using System;

namespace Gizmo.Web.Components
{
    public partial class Chip : CustomDOMComponentBase
    {
        #region FIELDS

        private bool _selected;
        private bool _isSelected;

        #endregion

        #region PROPERTIES

        [CascadingParameter]
        protected ChipGroup ChipGroup { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets if element is disabled.
        /// </summary>
        [Parameter]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;

                if (ChipGroup != null)
                {
                    ChipGroup.SelectItem(this, _isSelected);
                }
            }
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            _selected = IsSelected;

            if (ChipGroup != null)
            {
                ChipGroup.Register(this);

                if (_selected)
                {
                    ChipGroup.SelectItem(this, true);
                }
            }
        }

        public override void Dispose()
        {
            try
            {
                if (ChipGroup != null)
                {
                    ChipGroup.Unregister(this);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

        #region METHODS

        internal void SetSelected(bool selected)
        {
            if (IsDisabled)
                return;

            if (_selected == selected)
                return;

            _selected = selected;
            IsSelected = _selected;

            StateHasChanged();
        }

        internal bool GetSelected()
        {
            return _selected;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-chip")
                 .If("giz-chip--group-chip", () => ChipGroup != null)
                 .If("disabled", () => IsDisabled)
                 .If("selected", () => _selected)
                 .AsString();

        #endregion

    }
}