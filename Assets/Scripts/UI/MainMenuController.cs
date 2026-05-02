namespace HoldMyBeer.UI {
    public class MainMenuController {
        private readonly UINavigation nav;

        public MainMenuController(UINavigation nav) {
            this.nav = nav;
        }

        public void StartGame() {
            // TODO: call IAppActions.StartGame()
        }

        public void OpenSettings() {
            nav.Open(UIScreen.Settings);
        }

        public void OpenAbout() {
            nav.Open(UIScreen.About);
        }

        public void Quit() {
            // TODO: call IAppActions.Quit()
        }
    }
}