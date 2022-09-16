using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gizmo.Web.Components
{
    public partial class Button : ButtonBase
    {
        #region CONSTRUCTOR
        public Button()
        {
        }
        #endregion

        #region FIELDS

        private bool _selected;
        private bool _isSelected;
        private bool _canExecute = true;

        private ICommand _previousCommand;

        #endregion

        #region PROPERTIES

        [CascadingParameter]
        protected ButtonGroup ButtonGroup { get; set; }

        #region PUBLIC

        [Parameter]
        public ButtonVariants Variant { get; set; } = ButtonVariants.Fill;

        [Parameter]
        public bool IsFullWidth { get; set; }

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        [Parameter]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets label.
        /// </summary>
        [Parameter]
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
        public Icons? LeftSVGIcon { get; set; }

        [Parameter]
        public Icons? RightSVGIcon { get; set; }

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
        public bool StopPropagation { get; set; } = true;

        [Parameter]
        public decimal Progress { get; set; }

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

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            _canExecute = Command.CanExecute(CommandParameter);
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

        protected override async Task OnParametersSetAsync()
        {
            bool newCommand = !EqualityComparer<ICommand>.Default.Equals(_previousCommand, Command);

            if (newCommand)
            {
                if (_previousCommand != null)
                {
                    //Remove handler
                    _previousCommand.CanExecuteChanged -= Command_CanExecuteChanged;
                }
                if (Command != null)
                {
                    //Add handler
                    Command.CanExecuteChanged += Command_CanExecuteChanged;
                }
            }

            _previousCommand = Command;

            await base.OnParametersSetAsync();
        }

        public override void Dispose()
        {
            try
            {
                if (_previousCommand != null)
                {
                    //Remove handler
                    _previousCommand.CanExecuteChanged -= Command_CanExecuteChanged;
                }

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
            if (IsDisabled || !_canExecute)
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
                 .Add($"{Color.ToDescriptionString()}")
                 .If("giz-button--group-button", () => ButtonGroup != null)
                 .If("giz-button--fill", () => ButtonGroup == null && Variant == ButtonVariants.Fill)
                 .If("giz-button--outline", () => ButtonGroup == null && Variant == ButtonVariants.Outline)
                 .If("giz-button--text", () => ButtonGroup == null && Variant == ButtonVariants.Text)
                 .If("giz-button--progress", () => ButtonGroup == null && Variant == ButtonVariants.Progress)
                 .If("giz-button-full-width", () => IsFullWidth)
                 .If("giz-button-shadow", () => HasShadow)
                 .If("disabled", () => IsDisabled || !_canExecute)
                 .If("selected", () => _selected)
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 //TODO: A .If($"padding: 0 0.6rem;", () => string.IsNullOrEmpty(RightIcon) && !RightSVGIcon.HasValue && ChildContent == null)
                 .AsString();

        protected string ButtonIconLeft => new ClassMapper()
                .If("giz-button-icon-left", () => ChildContent != null || !string.IsNullOrEmpty(RightIcon))
                .AsString();

        protected string ButtonIconRight => new ClassMapper()
                .Add("giz-button-icon-right")
                .AsString();

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}