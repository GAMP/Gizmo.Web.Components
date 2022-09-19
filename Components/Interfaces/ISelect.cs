using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public interface ISelect<TItemType>
    {
        void Register(ISelectItem<TItemType> selectItem, TItemType value);

        void UpdateItem(ISelectItem<TItemType> selectItem, TItemType value);

        void Unregister(ISelectItem<TItemType> selectItem, TItemType value);

        Task SetSelectedItem(ISelectItem<TItemType> selectItem);
    }
}