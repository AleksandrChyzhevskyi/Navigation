using System;
using System.Collections.Generic;
using System.Linq;
using _Development.Scripts.Upgrade.Data;
using _Development.Scripts.Upgrade.Initialization;
using BLINK.RPGBuilder.Characters;
using BLINK.RPGBuilder.LogicMono;
using BLINK.RPGBuilder.Managers;
using UnityEngine;
using UnityEngine.AI;

namespace _Development.Scripts.Navigation
{
    public class PlayerNavigation : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;

        private Dictionary<TargetPoint, LevelPoint> _targetPoints;
        private List<LevelPoint> _currentTargetPoints;
        private NavMeshAgent _agentNew;
        private List<LevelPoint> _finishPoints;
        private LevelPoint _currentLevelPoint;
        private IDirectionArrow _directionArrow;

        private bool _isReady;
        private bool _isReactivateAgent;

        private void LateUpdate()
        {
            if (_isReady == false)
                return;

            CheckReactivateAgent(_isReactivateAgent);

            if (_currentLevelPoint.IsCompleted)
                UpdateFinishCurrentPoint();

            if (_isReactivateAgent == false && _isReady)
                _directionArrow.ShowDirection(transform.position);
        }

        private void OnDisable() =>
            Unsubscribe();

        public void Initialize(List<LevelPoint> finishPoints)
        {
            _agentNew = Instantiate(_agent, transform);
            Subscribe();

            FillFinishPoints(finishPoints);

            CheckTargetPoints();
            _currentLevelPoint = SetCurrentLevelPoint();
            
            if (CheckCurrentPoint()) 
                return;
            
            _directionArrow = new DirectionArrow(_agentNew, _currentLevelPoint.transform.position, _agentNew.transform);
            _isReady = true;
        }

        public void SetOffNavigation()
        {
            _isReady = false;
            _agentNew.gameObject.SetActive(false);
        }
        
        public void SetOnNavigation()
        {
            _isReady = true;
            _agentNew.gameObject.SetActive(true);
        }

        private void Subscribe() =>
            CombatEvents.PlayerDeathAccepted += ReactivationAgent;

        private void Unsubscribe() =>
            CombatEvents.PlayerDeathAccepted -= ReactivationAgent;

        private void ReactivationAgent()
        {
            SetOnNavigation();
            _isReactivateAgent = true;
        }

        private void UpdateFinishCurrentPoint()
        {
            SavePointCompleted();
            
            _currentLevelPoint = SetCurrentLevelPoint();
            
            if (CheckCurrentPoint()) 
                return;
            
            _directionArrow.ResetFinishPosition(_currentLevelPoint.transform.position);
        }

        private bool CheckCurrentPoint()
        {
            if (_currentLevelPoint != null) 
                return false;
            
            SetNotReady();
            return true;
        }

        public void CheckReactivateAgent(bool isReactivateAgent)
        {
            if (isReactivateAgent == false)
                return;

            CheckTargetPoints();
            _currentLevelPoint = SetCurrentLevelPoint();
            
            if (CheckCurrentPoint()) 
                return;
            
            ReactivateAgent();
            _directionArrow.ResetFinishPosition(_currentLevelPoint.transform.position);
            _isReactivateAgent = false;
        }

        private LevelPoint SetCurrentLevelPoint()
        {
            if (_currentTargetPoints != null)
            {
                foreach (LevelPoint levelPoint in _currentTargetPoints)
                    if (levelPoint.IsCompleted == false)
                        return levelPoint;
            }

            return _finishPoints.OrderBy(levelPoint => levelPoint.PriorityIndex)
                                .FirstOrDefault(levelPoint => levelPoint.IsCompleted == false);
        }

        private void ReactivateAgent()
        {
            _agentNew.gameObject.SetActive(false);
            _agentNew.gameObject.transform.position = Character.Instance.gameObject.transform.position;
            _agentNew.gameObject.SetActive(true);
            _directionArrow = new DirectionArrow(_agentNew, _currentLevelPoint.transform.position, _agentNew.transform);
        }

        private void FillFinishPoints(List<LevelPoint> finishPoints)
        {
            _targetPoints = new Dictionary<TargetPoint, LevelPoint>();
            _finishPoints = new List<LevelPoint>();

            foreach (LevelPoint point in finishPoints.OrderBy(levelPoint => levelPoint.PriorityIndex))
            {
                if (point.IsCompleted == false)
                    _finishPoints.Add(point);

                if (point.Target == TargetPoint.Default)
                    continue;

                if (_targetPoints.ContainsValue(point) == false)
                    _targetPoints.Add(point.Target, point);
            }
        }

        private void SetNotReady()
        {
            _isReady = false;
            _agentNew.gameObject.SetActive(false);
        }

        private void CheckTargetPoints()
        {
            _currentTargetPoints = new List<LevelPoint>();

            if (RPGBuilderEssentials.Instance.luckyWheelModelObject.CheckTryPaySpin())
            {
                LevelPoint levelPoint = _targetPoints[TargetPoint.Roulette];
                TryAddLevelPointToCurrentTargetPoints(levelPoint);
            }

            if (CheckUpgrade())
            {
                LevelPoint levelPoint = _targetPoints[TargetPoint.Upgrade];
                TryAddLevelPointToCurrentTargetPoints(levelPoint);
            }
        }

        private void TryAddLevelPointToCurrentTargetPoints(LevelPoint levelPoint)
        {
            if (levelPoint.IsCompleted)
                levelPoint.SetStateCompleted(false);

            if (_currentTargetPoints.Contains(levelPoint) == false)
                _currentTargetPoints.Add(levelPoint);
        }

        private bool CheckUpgrade()
        {
            foreach (var model in UpgradeInitialization.Instance.GetUpgradeModel())
            {
                UpgradeData.Upgrade modelUpgrade = model.NextUpgrade();
                if (InventoryManager.Instance.TryBuy(model.GetData().Currency.ID, modelUpgrade.UpgradeCost))
                    return true;
            }

            return false;
        }

        private void SavePointCompleted() =>
            PlayerPrefs.SetInt($"{_currentLevelPoint.NameLevelPoint}",
                Convert.ToUInt16(_currentLevelPoint.IsCompleted));
    }
}