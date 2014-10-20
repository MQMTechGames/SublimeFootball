using UnityEngine;
using System.Collections.Generic;
using MQMTech.AI.BT;

namespace MQMTech.AI.ST
{
    public class SituationTree
    {
        List<string> _situationWhiteList = new List<string>();
        public string Name { get; set; }

        Behavior _currentBehavior;

        Situation _root;

        public SituationTree(string name, Situation root)
        {
            Name = name;
            _root = root;
        }

        public void Tick()
        {
            BehaviorSituation nextBehavior;
            bool found = GetBehaviorSituation(out nextBehavior);
            if(found)
            {
                if(_currentBehavior != nextBehavior.Behavior)
                {
                    if(_currentBehavior != null)
                    {
                        _currentBehavior.OnReset(); // Maybe this should be OnAbort to terminate running states
                    }
                    
                    _currentBehavior = nextBehavior.Behavior;
                }
                else
                {
                    if(_currentBehavior != null && _currentBehavior.BehaviorStatus != Behavior.Status.RUNNING)
                    {
                        _currentBehavior.OnReset();
                    }
                }
            }

            if(_currentBehavior != null)
            {
                _currentBehavior.Tick();
            }
        }

        private bool GetBehaviorSituation(out BehaviorSituation oBehavior)
        {
            List<string> newWhiteList = new List<string>();
            bool found = _root.GetBehaviorSituation(out oBehavior, newWhiteList);

            if(found)
            {
                _situationWhiteList = newWhiteList;
            }

            return found;
        }

        public bool CheckSituationInWhiteList(string situationName)
        {
            if(_currentBehavior == null || _currentBehavior.BehaviorStatus != Behavior.Status.RUNNING)
            {
                return true;
            }

            return _situationWhiteList.Contains(situationName);
        }
    }

    public abstract class BaseSituation
    {
        public enum SituationStatus
        {
            FAILURE,
            SUCCESS,
        }

        protected SituationTree _st;

        public string Name{get; set;}

        public BaseSituation(string name)
        {
            Name = name;
        }

        protected SituationStatus _status;
        public SituationStatus Status
        { 
            get{ return _status; }
        }
        
        public virtual void OnInit(SituationTree st)
        {
            _st = st;
        }
    }

    public abstract class Condition : BaseSituation
    {
        public Condition(string name)
            : base(name)
        {}

        public abstract SituationStatus CheckCondition();
    }

    public abstract class Situation : BaseSituation
    {
        List<Condition> _conditions = new List<Condition>();
        
        public Situation(string name)
            : base(name)
        {}

        public override void OnInit(SituationTree st)
        {
            base.OnInit(st);

            for (int i = 0; i < _conditions.Count; ++i)
            {
                _conditions[i].OnInit(st);
            }
        }
        
        public void AddCondition(Condition condition)
        {
            _conditions.Add(condition);
        }

        protected SituationStatus CheckCondition()
        {
            bool isInWhiteList = _st.CheckSituationInWhiteList(Name);
            if(!isInWhiteList)
            {
                return SituationStatus.FAILURE;
            }

            for (int i = 0; i < _conditions.Count; ++i)
            {
                SituationStatus status = _conditions[i].CheckCondition();
                if(status != SituationStatus.SUCCESS)
                {
                    return status;
                }
            }

            return SituationStatus.SUCCESS;
        }

        public abstract bool GetBehaviorSituation(out BehaviorSituation oBehavior, List<string> newWhiteList);
        public virtual List<string> GetWhiteList() { return null; }
    }
    
    public class CompositeSituation : Situation
    {
        List<Situation> _situations = new List<Situation>();
        
        public CompositeSituation(string name)
            : base(name)
        {}

        public override void OnInit(SituationTree st)
        {
            base.OnInit(st);
            
            for (int i = 0; i < _situations.Count; ++i)
            {
                _situations[i].OnInit(st);
            }
        }
        public void AddSituation(Situation child)
        {
            _situations.Add(child);
        }
        
        public override bool GetBehaviorSituation(out BehaviorSituation oBehavior, List<string> newWhiteList)
        {
            oBehavior = null;

            SituationStatus status = CheckCondition();
            if(status != SituationStatus.SUCCESS)
            {
                return false;
            }

            List<string> whiteList = GetWhiteList();
            if(whiteList != null)
            {
                newWhiteList.AddRange(whiteList);
            }

            for (int i = 0; i < _situations.Count; ++i)
            {
                bool found = _situations[i].GetBehaviorSituation(out oBehavior, newWhiteList);
                if(found)
                {
                    return true;
                }
            }

            if(whiteList != null)
            {
                newWhiteList.RemoveRange(newWhiteList.Count - whiteList.Count -1, whiteList.Count);
            }

            return false;
        }
    }

    public class BehaviorSituation : Situation
    {
        public Behavior Behavior { get; private set; }
        
        public BehaviorSituation(string name)
            : base(name)
        {}
        
        public void SetBehavior(Behavior behavior)
        {
            Behavior = behavior;
        }
        
        public override bool GetBehaviorSituation(out BehaviorSituation oBehavior, List<string> newWhiteList)
        {
            oBehavior = null;

            SituationStatus status = CheckCondition();
            if(status != SituationStatus.SUCCESS)
            {
                return false;
            }

            List<string> whiteList = GetWhiteList();
            if(whiteList != null)
            {
                newWhiteList.AddRange(whiteList);
            }

            oBehavior = this;
            return true;
        }
    }
}
