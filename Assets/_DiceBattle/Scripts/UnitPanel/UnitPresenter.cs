namespace DiceBattle
{
    public class UnitPresenter
    {
        private readonly UnitModel _model;
        private readonly UnitView _view;

        public UnitPresenter(UnitModel model, UnitView view)
        {
            _model = model;
            _view = view;
        }

        public void Subscribe()
        {
            _model.CurrentHealth.ValueChanged += SetCurrentHealth;
        }

        public void Dispose()
        {
            _model.CurrentHealth.ValueChanged -= SetCurrentHealth;
        }

        public void Show() => _view.Construct(_model);

        private void SetCurrentHealth(int current, int previous) => _view.UpdateCurrentHealth(current);
    }
}