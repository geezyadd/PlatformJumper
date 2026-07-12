using System;
using Features.GameFlowStateMachineModule;
using Features.MovableModule.Scripts;
using Zenject;

namespace GameCore.Installers {
    public class DataInstaller : Installer<DataInstaller> {
        public override void InstallBindings() {
            Bind(typeof(GameFlowStateLifecycleEventClass));
            Bind(typeof(MovableModel));
        }

        public void Bind(Type type) {
            Container.Bind(type).AsSingle();
        }
    }
}