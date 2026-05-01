namespace HoldMyBeer.UI {
    public class CreditsController {
        private readonly UINavigation nav;

        public CreditsController(UINavigation nav) {
            this.nav = nav;
        }

        public void Back() {
            nav.Back();
        }
    }
}