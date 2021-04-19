using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    /// <summary>
    /// DataGrid column.
    /// </summary>
    /// <typeparam name="TItemType"></typeparam>
    public partial class DataGridColumn<TItemType> : CustomDOMComponentBase
    {
        #region CONSTRUCTOR

        #region PUBLIC
        
        public DataGridColumn():base()
        {
            ClassMapper.Add(() => "data-grid-column");
        }

        #endregion

        #endregion

        #region PROPERTIES

        #region PUBLIC

        /// <summary>
        /// Gets or sets field name of the data object.
        /// </summary>
        [Parameter()]
        public string Field
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets parent grid.
        /// </summary>
        [CascadingParameter(Name = "Parent")]
        public DataGrid<TItemType> Parent
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets header template.
        /// </summary>
        [Parameter()]
        public RenderFragment HeaderTemplate
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets template header.
        /// </summary>
        [Parameter()]
        public RenderFragment<TItemType> CellTemplate
        {
            get; set;
        }

        #endregion

        #endregion

        #region OVERRIDES

        #region PROTECTED

        protected override void OnInitialized()
        {
            Parent?.AddColumn(this);
        }

        #endregion

        #region PUBLIC

        public override void Dispose()
        {
            base.Dispose();

            Parent?.RemoveColumn(this);
        }

        #endregion

        #endregion

        #region EVENT HANDLERS

        #region PRIVATE

        protected ValueTask OnHeaderMouseEvent(MouseEventArgs args)
        {
            if (Parent == null)
                return ValueTask.CompletedTask;

            return Parent.OnHeaderRowMouseEvent(args, this);
        }

        #endregion

        #endregion
    }
}
