using Get.UI.Data;
using Get.Data.Properties;
using Get.Data.Bindings;
namespace Gtudios.UI.TooManyTabs;
class TooManyTabsViewSelector<T> : TemplateControl<UserControl>
{
    TooManyTabsStackView<T>? stackView;
    TooManyTabsMultiView<T>? multiView;
    TooManyTabsSingleView<T>? singleView;
    public Property<TooManyTabsItem<T>?> TooManyTabsItem { get; } = new(default);
    protected override void Initialize(UserControl rootElement)
    {
        void SetContent(TooManyTabsItem<T> item)
        {
            if (item is TooManyTabsStackItem<T> stackItem)
            {
                if (stackView is not null)
                {
                    stackView.TooManyTabsMultiItemProperty.Value = stackItem;
                }
                else
                {
                    stackView = new(_Parent, stackItem);
                }
                rootElement.Content = stackView;
            }
            else if (item is TooManyTabsMultiItem<T> multiItem)
            {
                if (multiView is not null)
                {
                    multiView.TooManyTabsMultiItem = multiItem;
                }
                else
                {
                    multiView = new(_Parent, multiItem);
                }
                rootElement.Content = multiView;
            }
            else if (item is TooManyTabsSingleItem<T> singleItem)
            {
                if (singleView is not null)
                {
                    singleView.TooManyTabsSingleItem = singleItem;
                }
                else
                {
                    singleView = new(_Parent, singleItem);
                }
                rootElement.Content = singleView;
            }
            else
            {
                rootElement.Content = null;
            }
        }
        SetContent(CurrentNode.CurrentValue);
        CurrentNode.ValueChanged += (_, @new) => SetContent(@new);
    }
    TooManyTabsView<T> _Parent;
    readonly IReadOnlyBinding<TooManyTabsItem<T>> CurrentNode;
    public TooManyTabsViewSelector(TooManyTabsView<T> Parent, IReadOnlyBinding<TooManyTabsItem<T>> CurrentNode)
    {
        _Parent = Parent;
        this.CurrentNode = CurrentNode;
    }
}