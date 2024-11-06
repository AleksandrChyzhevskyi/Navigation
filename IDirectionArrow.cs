using UnityEngine;
using UnityEngine.AI;

namespace _Development.Scripts.Navigation
{
    public interface IDirectionArrow
    {
        void ResetFinishPosition(Vector3 position);
        void ShowDirection(Vector3 transformPosition);
        void ResetAgent(NavMeshAgent agentNew);
    }
}