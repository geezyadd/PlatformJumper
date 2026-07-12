using Features.GameFlowStateMachineModule;
using Features.GameFlowStateMachineModule.Installers;
using Features.InputModule.Realization.Scripts;
using Zenject;

namespace GameCore.Installers {
    public class GlobalSceneInstaller : MonoInstaller<GlobalSceneInstaller> {
        public override void InstallBindings() {
            DataInstaller.Install(Container);
            GameFlowStateMachineModuleInstaller.Install(Container);
            InputModuleInstaller.Install(Container);
        }
    }
}
