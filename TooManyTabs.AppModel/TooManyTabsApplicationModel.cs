using System.Threading.Tasks;
using Gtudios.UI.Windowing;
namespace Gtudios.UI.TooManyTabs.AppModel;
public abstract class TooManyTabsApplicationModel<T>
{
    public abstract Task<T> CreateAsync();
    protected virtual void CreateWindow()
    {
        
    }
}