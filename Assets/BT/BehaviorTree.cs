using UnityEngine;
using System.Collections.Generic;

namespace MQMTech.AI.BT
{
    public class BehaviorTree
    {
        public string Name { get; private set; }

    	Behavior _root;
        public Behavior RootBehavior { get { return _root; } }
    	Behavior.Status _status;

        MemoryManager _memoryManager;
        public MemoryManager MemoryManager { get{ return _memoryManager; }}

        public BehaviorTree(string name)
        {
            Name = name;

            _memoryManager = new MemoryManager();
            _memoryManager.LocalMemory = new Memory();
        }

        public void SetAgentMemory(Memory memory)
        {
            _memoryManager.AgentMemory = memory;
        }

        public Memory GetAgentMemory()
        {
            return _memoryManager.AgentMemory;
        }

        public void SetLocalMemory(Memory memory)
        {
            _memoryManager.LocalMemory = memory;
        }

        public void SetSharedMemory(Memory memory)
        {
            _memoryManager.SharedMemory = memory;
        }

        public Memory GetSharedMemory()
        {
            return _memoryManager.SharedMemory;
        }

        public void SetGlobalMemory(Memory memory)
        {
            _memoryManager.GlobalMemory = memory;
        }

        public Memory GetGlobalMemory()
        {
            return _memoryManager.GlobalMemory;
        }

        public bool GetMemoryObject<T>(AIMemoryKey key, out T oVar)
    	{
            return _memoryManager.GetMemoryObject(key, out oVar);
    	}

        public void SetMemoryObject<T>(AIMemoryKey key, T oVar)
    	{
            _memoryManager.SetMemoryObject(key, oVar);
    	}

        public bool RemoveMemoryObject(AIMemoryKey key)
    	{
            return _memoryManager.RemoveMemoryObject(key);
    	}

        public bool ContainsMemoryObject(AIMemoryKey key)
    	{
            return _memoryManager.ContainsMemoryObject(key);
    	}

    	public void Tick()
    	{
    		if(_status != Behavior.Status.RUNNING)
    		{
    			_root.OnInitialize();
    		}

    		_status = _root.Tick();
    	}

    	public void Init(Behavior iBehavior)
    	{
    		_status = Behavior.Status.INVALID;
    		_root = iBehavior;

    		_root.OnInit(this);
    	}

    	public Behavior GetRootBehavior()
    	{
    		return _root;
    	}
    }

    public abstract class Behavior
    {
    	public enum Status
    	{
    		INVALID,
    		FAILURE,
    		SUCCESS,
    		RUNNING
    	}

    	protected Status _status;
    	public Status BehaviorStatus
    	{ 
    		get{ return _status; }
    	}

    	protected BehaviorTree _bt;
    	public string Name{get; set;}

    	public virtual void OnInit(BehaviorTree bt)
    	{
    		_bt = bt;
    		OnReset();
    	}

    	public virtual void OnReset()
    	{
    		_status = Status.INVALID;
    	}

    	public Status Tick()
    	{
    		if(_status == Status.INVALID)
    		{
    			OnInitialize();
    		}

    		_status = Update();

    		if(_status != Status.RUNNING)
    		{
    			OnTerminate();
    		}

    		return _status;
    	}

    	public virtual void OnInitialize(){}
    	public abstract Status Update();
    	public virtual void OnTerminate(){}

    	public virtual List<Behavior> GetChildren()
    	{
    		return null;
    	}
    }

    public abstract class Composite : Behavior
    {
    	protected List<Behavior> _children = new List<Behavior>();
    	protected int _childrenIdx;

    	public override void OnInit(BehaviorTree bt)
    	{
    		base.OnInit(bt);

    		foreach (Behavior behavior in _children) 
    		{
    			behavior.OnInit(bt);
    		}
    	}

    	public virtual Behavior AddChild(Behavior behavior)
    	{
    		_children.Add(behavior);

            return this;
    	}

    	public override void OnInitialize()
    	{
    		foreach (Behavior behavior in _children) 
    		{
    			behavior.OnReset();
    		}

    		_childrenIdx = 0;
    	}

    	public override List<Behavior> GetChildren()
    	{
    		return _children;
    	}
    }

    public class Parallel : Composite
    {
        public enum ParallelPolicy
        {
            SUCCESS_IF_ONE,
            SUCCESS_IF_ALL,
            
            FAILURE_IF_ONE,
            FAILURE_IF_ALL,
        }

        ParallelPolicy _successPolicy;
        ParallelPolicy _failurePolicy;

        int _numSuccess;
        int _numFailures;

        public override void OnInitialize()
        {
            base.OnInitialize();

            _numFailures = 0;
            _numSuccess = 0;
        }

        public Parallel(ParallelPolicy successPolicy, ParallelPolicy failurePolicy)
        {
            _successPolicy = successPolicy;
            _failurePolicy = failurePolicy;
        }

        public override Status Update()
        {
            for (int i = 0; i < _children.Count; ++i) 
            {
                if(   _children[i].BehaviorStatus == Status.INVALID
                   || _children[i].BehaviorStatus == Status.RUNNING
                   )
                {
                    Status status = _children[i].Tick();

                    if(status == Status.FAILURE)
                    {
                        _numFailures++;
                    }
                    else if(status == Status.SUCCESS)
                    {
                        _numSuccess++;
                    }
                }
            }

            if(_numSuccess > 0 && _successPolicy == ParallelPolicy.SUCCESS_IF_ONE)
            {
                return Status.SUCCESS;
            }
            else if(_numSuccess == _children.Count && _successPolicy == ParallelPolicy.SUCCESS_IF_ALL)
            {
                return Status.SUCCESS;
            }

            if(_numFailures > 0 && _failurePolicy == ParallelPolicy.FAILURE_IF_ONE)
            {
                return Status.FAILURE;
            }
            else if(_numFailures == _children.Count && _failurePolicy == ParallelPolicy.FAILURE_IF_ALL)
            {
                return Status.FAILURE;
            }

            return Status.RUNNING;
        }
    }

    public class Sequence : Composite
    {
    	public override Status Update()
    	{
    		for (int i = _childrenIdx; i < _children.Count; ++i) 
    		{
    			Status status = _children[i].Tick();

                if(status == Status.FAILURE)
                {
                    return status;
                }

    			if(status == Status.RUNNING)
    			{
    				_childrenIdx = i;
                    return status;
    			}
    		}

    		return Status.SUCCESS;
    	}
    }

    public class ActiveSequence : Composite
    {
    	public override Status Update()
    	{
    		for (int i = 0; i < _children.Count; ++i) 
    		{
    			if(i < _childrenIdx)
    			{
    				_children[i].OnReset();
    			}

    			Status status = _children[i].Tick();
    			
                if(status == Status.FAILURE)
                {
                    return status;
                }
                
                if(status == Status.RUNNING)
                {
                    _childrenIdx = i;
                    return status;
                }
    		}
    		
    		return Status.SUCCESS;
    	}
    }

    public class Selector : Composite
    {
    	public override Status Update()
    	{
    		for (int i = _childrenIdx; i < _children.Count; ++i) 
    		{
    			Status status = _children[i].Tick();
    			
    			if(status != Status.FAILURE)
    			{
    				_childrenIdx = i;

    				return status;
    			}
    		}

    		return Status.FAILURE;
    	}
    }

    public class ActiveSelector : Composite
    {
    	public override Status Update()
    	{
    		for (int i = 0; i < _children.Count; ++i) 
    		{
    			if(i < _childrenIdx)
    			{
    				_children[i].OnReset();
    			}

    			Status status = _children[i].Tick();
    			
    			if(status != Status.FAILURE)
    			{
                    if(i < _childrenIdx)
                    {
                        _children[_childrenIdx].OnReset();
                    }

    				_childrenIdx = i;
    				return _status;
    			}
    		}

    		return Status.FAILURE;
    	}
    }

    public class RandomSelector : Composite
    {
        public override void OnInitialize()
        {
            base.OnInitialize();

            _childrenIdx = UnityEngine.Random.Range(0, _children.Count);
        }

        public override Status Update()
        {
            return _children[_childrenIdx].Tick();
        }
    }

    public class ProbabilitySelector : Composite
    {
        List<AIMemoryKey> _probabilityKeys = new List<AIMemoryKey>();
        float[]_probabilities;
        float[] _probabilitiesStart;
        float[] _probabilitiesEnd;

        public void AddChild(Behavior child, AIMemoryKey probabilityKey)
        {
            base.AddChild(child);
            _probabilityKeys.Add(probabilityKey);
        }

        public override void OnInit(BehaviorTree bt)
        {
            base.OnInit(bt);

            _probabilitiesStart = new float[_probabilityKeys.Count];
            _probabilitiesEnd = new float[_probabilityKeys.Count];
            _probabilities = new float[_probabilityKeys.Count];
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            CalculateNormalizedProbabilities();
            SelectRandomChild();
        }

        void CalculateNormalizedProbabilities()
        {
            float totalProbability = 0f;
            
            for (int i = 0; i < _probabilityKeys.Count; ++i)
            {
                _probabilities[i] = GetProbabilityFromMemory(_probabilityKeys[i]);
                totalProbability += _probabilities[i];
                //Debug.Log(_probabilityKeys[i].Name + ": " + probability);
            }
            
            float currentProbability = 0f;
            for (int i = 0; i < _probabilities.Length; ++i)
            {
                float probability = _probabilities[i];
                
                _probabilitiesStart[i] = currentProbability;
                _probabilitiesEnd[i] = currentProbability + (probability / totalProbability);
                
                currentProbability = _probabilitiesEnd[i];
            }
        }

        float GetProbabilityFromMemory(AIMemoryKey key)
        {
            float probability = 1f;
            bool isOk = _bt.GetMemoryObject(key, out probability);
            DebugUtils.Assert(isOk, "probability is not found");
            
            return probability;
        }

        void SelectRandomChild()
        {
            UnityEngine.Random.seed = (int) Time.timeSinceLevelLoad;

            float probabilityNorm = UnityEngine.Random.Range(0f, 10000f) * 0.0001f;
            for (int i = 0; i < _probabilities.Length; ++i)
            {
                bool isValid = probabilityNorm >= _probabilitiesStart[i] && probabilityNorm <= _probabilitiesEnd[i];
                if(isValid)
                {
                    _childrenIdx = i;
                    break;
                }
            }
        }
        
        public override Status Update()
        {
            return _children[_childrenIdx].Tick();
        }
    }

    public abstract class Decorator : Behavior
    {
    	protected Behavior _child;

    	List<Behavior> _children = null;
    	protected List<Behavior> Children
    	{
    		get
    		{  
    			if(_children == null)
    			{
    				_children = new List<Behavior>();
    				_children.Add(_child);
    			}

    			return _children;
    		}
    	}

    	public override void OnInit(BehaviorTree bt)
    	{
    		base.OnInit(bt);
    		_child.OnInit(bt);
    	}

        public override void OnInitialize()
        {
            base.OnInitialize();
            _child.OnReset();
        }

    	public Behavior SetChild(Behavior behavior)
    	{
    		_child = behavior;

            return this;
    	}

    	public override List<Behavior> GetChildren()
    	{
    		return Children;
    	}
    }

    public class UntilFail : Decorator
    {
    	public override Status Update()
    	{
    		Status childStatus = _child.Tick();
    		if(childStatus == Status.FAILURE)
    		{
                return Status.SUCCESS;
    		}

            if(childStatus != Status.RUNNING)
            {
                _child.OnReset();
            }

            return Status.RUNNING;
    	}
    }

    public class UntilSuccess : Decorator
    {
        public override Status Update()
        {
            Status childStatus = _child.Tick();
            if(childStatus == Status.SUCCESS)
            {
                return Status.SUCCESS;
            }

            if(childStatus != Status.RUNNING)
            {
                _child.OnReset();
            }
            
            return Status.RUNNING;
        }
    }

    public class Inverter : Decorator
    {
    	public override Status Update()
    	{
    		_status = _child.Tick();

    		if(_status == Status.SUCCESS)
    		{
    			return Status.FAILURE;
    		}

    		if(_status == Status.FAILURE)
    		{
    			return Status.SUCCESS;
    		}

    		return _status;
    	}
    }

    public class Succeder : Decorator
    {
    	public override Status Update()
    	{
    		_status = _child.Tick();
    		
    		if(_status != Status.RUNNING)
    		{
    			_status = Status.SUCCESS;
    		}
    		
    		return _status;
    	}
    }

    public class Failer : Decorator
    {
    	public override Status Update()
    	{
    		_status = _child.Tick();
    		
    		if(_status != Status.RUNNING)
    		{
    			_status = Status.FAILURE;
    		}
    		
    		return _status;
    	}
    }

    public class RunForever : Behavior
    {
    	public override Status Update()
    	{
    		_status = Status.RUNNING;

    		return _status;
    	}
    }

    public class TimeWaiter : Behavior
    {
    	float _waitTime;
    	float _endTime;

    	public TimeWaiter(float time)
    	{
    		_waitTime = time;
    		_endTime = 0f;
    	}

    	public override void OnInitialize ()
    	{
    		base.OnInitialize ();

    		_endTime = Time.timeSinceLevelLoad + _waitTime;
    	}

    	public override Status Update()
    	{
    		if(_endTime < Time.timeSinceLevelLoad)
    		{
    			_status = Status.SUCCESS;
    		}
    		else
    		{
    			_status = Status.RUNNING;
    		}

    		_status = _status;
    		return _status;
    	}
    }

    public class Success : Behavior
    {
    	public override Status Update()
    	{
    		_status = Status.SUCCESS;
    		return _status;
    	}
    }

    public class Failure : Behavior
    {
    	public override Status Update()
    	{
    		_status = Status.FAILURE;
    		return _status;
    	}
    }

    public class CheckMemoryVar : Behavior
    {
    	AIMemoryKey _varName;

        public CheckMemoryVar(AIMemoryKey memoryVarName)
    	{
    		_varName = memoryVarName;
    	}

    	public override Status Update()
    	{
    		DebugUtils.Assert(_bt!=null, "_bt!=null");
    		bool containsVar = _bt.ContainsMemoryObject(_varName);
            return containsVar ? Status.SUCCESS : Status.FAILURE;
    	}
    }

    public class CheckNotNullMemoryVar : Behavior
    {
        AIMemoryKey _varName;
        
        public CheckNotNullMemoryVar(AIMemoryKey memoryVarName)
        {
            _varName = memoryVarName;
        }
        
        public override Status Update()
        {
            System.Object obj = null;
            _bt.GetMemoryObject<System.Object>(_varName, out obj);
            return obj != null ? Status.SUCCESS : Status.FAILURE;
        }
    }

    public class CheckNullMemoryVar : Behavior
    {
        AIMemoryKey _varName;
        
        public CheckNullMemoryVar(AIMemoryKey memoryVarName)
        {
            _varName = memoryVarName;
        }
        
        public override Status Update()
        {
            System.Object obj = null;
            _bt.GetMemoryObject<System.Object>(_varName, out obj);
            return obj == null ? Status.SUCCESS : Status.FAILURE;
        }
    }

    public class CheckMemoryType<T> : Behavior
    {
        AIMemoryKey _varName;

        public CheckMemoryType(AIMemoryKey memoryVarName)
    	{
    		_varName = memoryVarName;
    	}
    	
    	public override Status Update()
    	{
    		DebugUtils.Assert(_bt!=null, "_bt!=null");

    		T memoryObj;
    		bool isOk = _bt.GetMemoryObject<T>(_varName, out memoryObj);
    		if(!isOk)
    		{
    			return Status.FAILURE;
    		}

    		return memoryObj is T ? Status.SUCCESS : Status.FAILURE;
    	}
    }

    public class SetMemoryVar<T> : Behavior
    {
        AIMemoryKey _varName;
        T _var;
        
        public SetMemoryVar(AIMemoryKey memoryVarName, T var)
        {
            _varName = memoryVarName;
            _var = var;
        }
        
        public override Status Update()
        {
            DebugUtils.Assert(_bt!=null, "_bt!=null");
            
            _bt.SetMemoryObject<T>(_varName, _var);

            return Status.SUCCESS;
        }
    }

    public class RemoveMemoryVar : Behavior
    {
        AIMemoryKey _varName;
    	
        public RemoveMemoryVar(AIMemoryKey memoryVarName)
    	{
    		_varName = memoryVarName;
    	}
    	
    	public override Status Update()
    	{
    		bool containsVar = _bt.RemoveMemoryObject(_varName);
    		return Status.SUCCESS;
    	}
    }

    public class CheckAreEqualMemoryVars<T> : Behavior
    {
        AIMemoryKey _varNameA;
        AIMemoryKey _varNameB;
        
        public CheckAreEqualMemoryVars(AIMemoryKey varNameA, AIMemoryKey varNameB)
        {
            _varNameA = varNameA;
            _varNameB = varNameB;
        }
        
        public override Status Update()
        {
            T objA = default(T);
            bool isOk = _bt.GetMemoryObject<T>(_varNameA, out objA);
//            if(!isOk)
//                Debug.Log( _varNameA.Name + " not found");
            
            T objB = default(T);
            bool isOk2 = _bt.GetMemoryObject<T>(_varNameB, out objB);
//            if(!isOk2)
//                Debug.Log( _varNameB.Name + " not found");

//            if(_varNameA.Name == "IsBallControlled")
//            {
//                Debug.Log("objA: " + objA.ToString() + ", objB: " + objB.ToString() + ", areEquals: " + objA.Equals(objB));
//            }

            if(objA == null || objB == null)
            {
                return Status.FAILURE;
            }

            return objA.Equals(objB) ? Status.SUCCESS : Status.FAILURE;
        }
    }

    public class CheckAreEqualMemoryVars : CheckAreEqualMemoryVars<System.Object>
    {
        public CheckAreEqualMemoryVars(AIMemoryKey varNameA, AIMemoryKey varNameB)
            :base(varNameA, varNameB)
        {
        }
    }

    public class CheckAreEqualMemoryVarWithValue<T> : Behavior
    {
        AIMemoryKey _varNameA;
        T _value;
        
        public CheckAreEqualMemoryVarWithValue(AIMemoryKey varNameA, T value)
        {
            _varNameA = varNameA;
            _value = value;
        }
        
        public override Status Update()
        {
            T objA = default(T);
            bool isOk = _bt.GetMemoryObject<T>(_varNameA, out objA);
            
            if(objA == null || _value == null)
            {
                return Status.FAILURE;
            }
            
            return objA.Equals(_value) ? Status.SUCCESS : Status.FAILURE;
        }
    }

    public class CopyMemoryVar : Behavior
    {
        AIMemoryKey _source;
        AIMemoryKey _target;

        public CopyMemoryVar(AIMemoryKey source, AIMemoryKey target)
    	{
    		_source = source;
    		_target = target;
    	}
    	
    	public override Status Update()
    	{
    		System.Object objA = null;
    		_bt.GetMemoryObject<System.Object>(_source, out objA);
    		
    		_bt.SetMemoryObject<System.Object>(_target, objA);
    		
    		return Status.SUCCESS;
    	}
    }

    public class MoveMemoryVar : Behavior
    {
        AIMemoryKey _source;
        AIMemoryKey _target;
        
        public MoveMemoryVar(AIMemoryKey source, AIMemoryKey target)
        {
            _source = source;
            _target = target;
        }
        
        public override Status Update()
        {
            System.Object objA = null;
            _bt.GetMemoryObject<System.Object>(_source, out objA);
            
            _bt.SetMemoryObject<System.Object>(_target, objA);

            _bt.RemoveMemoryObject(_source);
            
            return Status.SUCCESS;
        }
    }

    public class LogAction : Behavior
    {
        string _msg;
        
        public LogAction(string msg)
        {
            _msg = msg;
        }
        
        public override Status Update()
        {
            Debug.Log(_msg);
            
            return Status.SUCCESS;
        }
    }

    public class LogVarAction : Behavior
    {
        AIMemoryKey _key;
        
        public LogVarAction(AIMemoryKey key)
        {
            _key = key;
        }
        
        public override Status Update()
        {
            System.Object obj;
            bool isOK = _bt.GetMemoryObject<System.Object>(_key, out obj);
            DebugUtils.Assert(isOK, _key.Name + " does NOT exist");
            DebugUtils.Assert(obj != null, _key.Name + "is NULL");
            if(obj != null)
            {
                Debug.Log(_key.Name + ": " + obj.ToString());
            }
            
            return Status.SUCCESS;
        }
    }

    public class GateMaxNumber : Decorator
    {
        public enum GatePolicy
        {
            FAIL_IF_UNAVAILABLE,
            WAIT_TILL_AVAILABLE,
        }

        GatePolicy _policy;
        AIMemoryKey _variableKey;
        int _maxNumber;
        bool _hasAccessed;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _hasAccessed = false;
        }
        
        public GateMaxNumber(AIMemoryKey variableKey, int maxNumber, GatePolicy policy)
        {
            _policy = policy;
            _variableKey = variableKey;
            _maxNumber = maxNumber;
        }
        
        public override Status Update()
        {
            if(!_hasAccessed)
            {
                int counter = 0;
                _bt.GetMemoryObject(_variableKey, out counter);

                if(counter >= _maxNumber)
                {
                    if(_policy == GatePolicy.FAIL_IF_UNAVAILABLE)
                    {
                        return Status.FAILURE;
                    }

                    return Status.RUNNING;
                }

                _bt.SetMemoryObject(_variableKey, ++counter);
                _hasAccessed = true;
            }

            return _child.Tick();
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            if(_hasAccessed)
            {
                int counter = 0;
                _bt.GetMemoryObject(_variableKey, out counter);
                _bt.SetMemoryObject(_variableKey, --counter);
            }
        }
    }

    public class SetCoolDownTime : Behavior
    {
        AIMemoryKey _timerKey;
        AIMemoryKey _waitTimeKey;

        public SetCoolDownTime(AIMemoryKey timerKey, AIMemoryKey waitTimeKey)
        {
            _timerKey = timerKey;
            _waitTimeKey = waitTimeKey;
        }

        public override Status Update()
        {
            float waitTime = -1f;
            _bt.GetMemoryObject(_waitTimeKey, out waitTime);

            float newEndTime = Time.timeSinceLevelLoad + waitTime;
            if(waitTime < 0f)
            {
                newEndTime = 0f;
            }

            _bt.SetMemoryObject(_timerKey, newEndTime);

            return Status.SUCCESS;
        }
    }

    public class CoolDown : Decorator
    {
        public enum CoolDownPolicy
        {
            FAIL_IF_UNAVAILABLE,
            WAIT_TILL_AVAILABLE,
        }
        
        CoolDownPolicy _policy;
        AIMemoryKey _timerKey;
        bool _hasAccessed;
        
        public override void OnInitialize()
        {
            base.OnInitialize();
            _hasAccessed = false;
        }
        
        public CoolDown(AIMemoryKey timerKey, CoolDownPolicy policy)
        {
            _timerKey = timerKey;
            _policy = policy;
        }
        
        public override Status Update()
        {
            if(!_hasAccessed)
            {
                float timer = 0;
                _bt.GetMemoryObject(_timerKey, out timer);

                if(timer >= Time.timeSinceLevelLoad)
                {
                    if(_policy == CoolDownPolicy.FAIL_IF_UNAVAILABLE)
                    {
                        return Status.FAILURE;
                    }
                    
                    return Status.RUNNING;
                }
                
                _hasAccessed = true;
            }
            
            return _child.Tick();
        }
    }
}
