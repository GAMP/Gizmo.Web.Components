using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public class GizInputBase<TValue> : CustomDOMComponentBase
    {
        #region FIELDS

        //private Guid _guid = Guid.NewGuid();
        private Expression<Func<TValue>> _lastValueExpression;
        protected FieldIdentifier _fieldIdentifier;
        protected EditContext _lastEditContext;
        protected bool _isValid = true;
        protected string _validationMessage;

        #endregion

        [CascadingParameter]
        protected EditContext EditContext { get; set; } = default!;

        #region PROPERTIES

        [Parameter]
        public Expression<Func<TValue>> ValueExpression { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        [Parameter]
        public bool IsHidden { get; set; }

        #endregion

        #region OVERRIDE

        protected override void OnParametersSet()
        {
            if (ValueExpression != null && ValueExpression != _lastValueExpression)
            {
                _fieldIdentifier = FieldIdentifier.Create(ValueExpression);
                _lastValueExpression = ValueExpression;
            }

            if (EditContext != _lastEditContext)
            {
                RemoveValidationHandlers();

                if (EditContext != null)
                {
                    //var fieldName = ValueExpression != null ? ValueExpression.ToString() : "";
                    //InvokeVoidAsync("writeLine", $"{_guid} OnParametersSet EditContext {fieldName}");

                    EditContext.OnValidationRequested += OnValidationRequested;
                    EditContext.OnValidationStateChanged += OnValidationStateChanged;
                    _lastEditContext = EditContext;
                }
            }

            base.OnParametersSet();
        }

        #endregion

        #region EVENTS

        private void OnValidationRequested(object sender, ValidationRequestedEventArgs e)
        {
            Validate();
        }

        private void OnValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
        {
            if (EditContext != null && !_fieldIdentifier.Equals(default(FieldIdentifier)))
            {
                var errors = EditContext.GetValidationMessages(_fieldIdentifier).ToList();
                if (errors.Count > 0)
                {
                    _isValid = false;
                    _validationMessage = String.Join(" ", errors);
                }
                else
                {
                    _isValid = true;
                    _validationMessage = null;
                }
                StateHasChanged();
            }
        }

        #endregion

        #region METHODS

        private void RemoveValidationHandlers()
        {
            if (_lastEditContext != null)
            {
                _lastEditContext.OnValidationRequested -= OnValidationRequested;
                _lastEditContext.OnValidationStateChanged -= OnValidationStateChanged;

                //TODO: A ??? ValidationMessageStore.Clear();
            }
        }

        protected void NotifyFieldChanged()
        {
            if (EditContext != null && !_fieldIdentifier.Equals(default(FieldIdentifier)))
            {
                EditContext.NotifyFieldChanged(_fieldIdentifier);
            }
        }

        #endregion

        public override void Dispose()
        {
            RemoveValidationHandlers();

            base.Dispose();
        }

        public virtual void Validate()
        {

        }
    }
}