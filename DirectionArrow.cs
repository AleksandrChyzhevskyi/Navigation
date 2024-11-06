using System;
using UnityEngine;
using UnityEngine.AI;

namespace _Development.Scripts.Navigation
{
    public class DirectionArrow : IDirectionArrow
    {
        private const int LongIndexPath = 2;
        private const int ShortIndexPath = 1;

        private NavMeshAgent _agent;
        private readonly Transform _arrowPosition;
        private NavMeshPath _navMeshPath;

        private Vector3 _finishPoint;

        public DirectionArrow(NavMeshAgent agent, Vector3 finishPoint, Transform arrowPosition)
        {
            _navMeshPath = new NavMeshPath();

            _agent = agent;
            _finishPoint = finishPoint;
            _arrowPosition = arrowPosition;
        }

        public void ShowDirection(Vector3 position)
        {
            TryCalculatePath();
            SetLookArrow();
            SetPosition(position);
        }

        public void ResetFinishPosition(Vector3 position)
        {
            ClearPath();
            _finishPoint = position;
        }

        public void ResetAgent(NavMeshAgent agentNew) => 
            _agent = agentNew;

        private void TryCalculatePath()
        {
            if (_agent.CalculatePath(_finishPoint, _navMeshPath) == false)
                throw new Exception("The path cannot be built");
        }
        
        private void SetLookArrow() =>
            _arrowPosition.LookAt(GetPositionInPath(_navMeshPath));

        private Vector3 GetPositionInPath(NavMeshPath path) =>
            path.corners.Length > LongIndexPath
                ? path.corners[LongIndexPath]
                : path.corners[ShortIndexPath];

        private void ClearPath() =>
            _navMeshPath = new NavMeshPath();

        private void SetPosition(Vector3 position) =>
            _arrowPosition.position = position;
    }
}