using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gizmo.Web.Components
{
    public partial class Button : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public Button()
        {
        }
        #endregion

        #region FIELDS

        private bool _selected;
        private bool _isSelected;

        #endregion

        #region PROPERTIES

        [CascadingParameter]
        protected ButtonGroup ButtonGroup { get; set; }

        #region PUBLIC

        [Parameter()]
        public ButtonVariants Variant { get; set; } = ButtonVariants.Fill;

        /// <summary>
        /// Gets or sets button size.
        /// </summary>
        [Parameter()]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        /// <summary>
        /// Gets or sets element name.
        /// </summary>
        [Parameter()]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets if element is disabled.
        /// </summary>
        [Parameter()]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool IsFullWidth { get; set; }

        /// <summary>
        /// Gets or sets element type.
        /// </summary>
        [Parameter()]
        public string Type { get; set; } = "button";

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        [Parameter()]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets label.
        /// </summary>
        [Parameter()]
        public string Label { get; set; }

        /// <summary>
        /// Inline label of Button.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string LeftIcon { get; set; }

        [Parameter]
        public string RightIcon { get; set; }

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

                if (ButtonGroup != null)
                {
                    ButtonGroup.SelectItem(this, _isSelected);
                }
            }
        }

        [Parameter]
        public ICommand Command { get; set; }

        [Parameter]
        public object CommandParameter { get; set; }

        #endregion

        #endregion

        #region EVENTS

        protected Task OnClickButtonHandler(MouseEventArgs args)
        {
            if (ButtonGroup != null && !ButtonGroup.IsDisabled)
            {
                ButtonGroup.SelectItem(this, !_selected);
            }

            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }

            return Task.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            _selected = IsSelected;

            if (ButtonGroup != null)
            {
                ButtonGroup.Register(this);

                if (_selected)
                {
                    ButtonGroup.SelectItem(this, true);
                }
            }
        }

        public override void Dispose()
        {
            try
            {
                if (ButtonGroup != null)
                {
                    ButtonGroup.Unregister(this);
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
                 .Add("giz-button")
                 .Add($"giz-button--{Size.ToDescriptionString()}")
                 .If("giz-button--group-button", () => ButtonGroup != null)
                 .If("giz-button--fill", () => ButtonGroup == null && Variant == ButtonVariants.Fill)
                 .If("giz-button--outline", () => ButtonGroup == null && Variant == ButtonVariants.Outline)
                 .If("giz-button--text", () => ButtonGroup == null && Variant == ButtonVariants.Text)
                 .If("giz-button--icon", () => ButtonGroup == null && Variant == ButtonVariants.Icon)
                 .If("giz-button-full-width", () => IsFullWidth)
                 .If("disabled", () => IsDisabled)
                 .If("selected", () => _selected)
                 .AsString();

        protected string ButtonIconLeft => new ClassMapper()
                .If("giz-button-icon-left", () => ChildContent != null || !string.IsNullOrEmpty(RightIcon))
                .AsString();

        protected string ButtonIconRight => new ClassMapper()
                .Add("giz-button-icon-right")
                .AsString();

        #endregion

    }
}