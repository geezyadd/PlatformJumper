using Zenject;

namespace Features.TowerGenerationModule.Scripts {
    public class TowerGenerationInstaller : Installer<TowerGenerationInstaller> {
        public override void InstallBindings() {
            Container.Bind<TowerSegmentManager>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}
