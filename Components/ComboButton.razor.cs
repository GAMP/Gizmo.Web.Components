using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gizmo.Web.Components
{
    public partial class ComboButton : ButtonBase, ISelect<int>
    {
        #region CONSTRUCTOR
        public ComboButton()
        {
        }
        #endregion

        #region FIELDS

        private Dictionary<int, SelectItem<int>> _items = new Dictionary<int, SelectItem<int>>();
        private SelectItem<int> _selectedItem;
        private List _popupContent;

        private bool _isOpen;
        private double _popupX;
        private double _popupY;
        private double _popupWidth;

        private bool _canExecute = true;

        private ICommand _previousCommand;

        #endregion

        #region PROPERTIES

        #region PUBLIC

        [Parameter]
        public ButtonVariants Variant { get; set; } = ButtonVariants.Fill;

        [Parameter]
        public bool IsFullWidth { get; set; }

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
        public Icons? LeftSVGIcon { get; set; }

        [Parameter]
        public bool StopPropagation { get; set; } = true;

        #endregion

        #endregion

        #region EVENTS

        protected Task OnClickButtonHandler(MouseEventArgs args)
        {
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
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-button")
                 .Add($"giz-button--{Size.ToDescriptionString()}")
                 .Add($"{Color.ToDescriptionString()}")
                 .If("giz-button--fill", () => Variant == ButtonVariants.Fill)
                 .If("giz-button--outline", () => Variant == ButtonVariants.Outline)
                 .If("giz-button--text", () => Variant == ButtonVariants.Text)
                 .If("giz-button-full-width", () => IsFullWidth)
                 .If("giz-button-shadow", () => HasShadow)
                 .If("disabled", () => IsDisabled || !_canExecute)
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

        public void Register(SelectItem<int> selectItem, int value)
        {
            _items[value] = selectItem;
        }

        public void UpdateItem(SelectItem<int> selectItem, int value)
        {
            //var actualItem = _items.Where(a => a.Value == selectItem).FirstOrDefault();
            //if (!actualItem.Equals(default(KeyValuePair<int, SelectItem<int>>)) && actualItem.Key != null)
            //{
            //    _items.Remove(actualItem.Key);
            //}

            //_items[value] = selectItem;
        }

        public void Unregister(SelectItem<int> selectItem, int value)
        {
            //var actualItem = _items.Where(a => a.Value == selectItem).FirstOrDefault();
            //if (!actualItem.Equals(default(KeyValuePair<int, SelectItem<int>>)))
            //{
            //    _items.Remove(actualItem.Key);
            //}
        }

        public Task SetSelectedItem(SelectItem<int> selectItem)
        {
            //bool requiresRefresh = _isOpen;

            //_isOpen = false;

            //if (_selectedItem == selectItem)
            //{
            //    if (requiresRefresh)
            //        StateHasChanged();

            //    return Task.CompletedTask;
            //}

            //_selectedItem = selectItem;

            //_hasParsingErrors = false;
            //_parsingErrors = String.Empty;

            //StateHasChanged();

            //if (selectItem != null)
            //    return SetSelectedValue(selectItem.Value);
            //else
            //    return SetSelectedValue(default(TValue));

            throw new NotImplementedException();
        }
    }
}