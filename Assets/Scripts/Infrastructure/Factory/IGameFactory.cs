using Roguelike.Infrastructure.Services;
using Roguelike.Infrastructure.States;
using UnityEngine;

namespace Roguelike.Infrastructure.Factory
{
    public interface IGameFactory : IService
    {
        GameObject CreatePlayer(Transform playerInitialPoint);
        GameObject CreateHud(GameObject player, bool createMiniMap);
        GameObject GenerateLevel();
        void CreateCharacterSelectionMode();
    }
}