using ContentLoading;

namespace ZooWorld.UI
{
    public class GameUIManager : UIManager<GameUIType>
    {
        public GameUIManager(UIContainer container, UIConfig config, IContentLoader loader)
            : base(container, config, loader) { }
    }
}