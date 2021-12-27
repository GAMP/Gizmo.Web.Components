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

        private Expression<Func<TValue>> _lastValueExpression;
        private FieldIdentifier _fieldIdentifier;
        private EditContext _lastEditContext;
        protected bool _isValid = true;
        protected string _validationMessage;

        #endregion

        [CascadingParameter]
        protected EditContext EditContext { get; set; } = default!;

        ValidationMessageStore ValidationMessageStore;

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
                    ValidationMessageStore = new ValidationMessageStore(EditContext);

                    EditContext.OnValidationRequested += OnValidationRequested;
                    EditContext.OnValidationStateChanged += OnValidationStateChanged;
                    _lastEditContext = EditContext;
                }
            }          

            base.OnParametersSet();
        }

        private void OnValidationRequested(object sender, ValidationRequestedEventArgs e)
        {
            Validate(_fieldIdentifier, ValidationMessageStore);
        }

        private void RemoveValidationHandlers()
        {
            if (_lastEditContext != null)
            {
                _lastEditContext.OnValidationRequested -= OnValidationRequested;
                _lastEditContext.OnValidationStateChanged -= OnValidationStateChanged;

                ValidationMessageStore.Clear();
            }
        }

        private void OnValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
        {
            if (EditContext != null && !_fieldIdentifier.Equals(default(FieldIdentifier)))
            {
                var errors = EditContext.GetValidationMessages(_fieldIdentifier).ToList();
                if (errors.Count > 0)
                {
                    _isValid = false;
                    _validationMessage = errors.FirstOrDefault();
                }
                else
                {
                    _isValid = true;
                    _validationMessage = null;
                }
                StateHasChanged();
            }
        }

        public override void Dispose()
        {
            RemoveValidationHandlers();

            base.Dispose();
        }

        public virtual void Validate(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {

        }
    }
}