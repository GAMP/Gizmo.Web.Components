using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public interface ISelect<TItemType>
    {
        void Register(SelectItem<TItemType> selectItem, TItemType value);

        void Update(SelectItem<TItemType> selectItem, TItemType value);

        void Unregister(SelectItem<TItemType> selectItem, TItemType value);

        Task SetSelectedItem(SelectItem<TItemType> selectItem);
    }
}