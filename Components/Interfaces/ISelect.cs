using System.Threading.Tasks;

namespace Gizmo.Web.Components
{
    public interface ISelect<TItemType>
    {
        void Register(SelectItem<TItemType> selectItem);

        void Unregister(SelectItem<TItemType> selectItem);

        Task SetSelectedItem(SelectItem<TItemType> selectItem);
    }
}