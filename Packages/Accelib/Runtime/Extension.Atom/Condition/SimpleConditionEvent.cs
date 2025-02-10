using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Extension.Atom.Condition
{
    public class SimpleConditionEvent : MonoBehaviour
    {
        [Header("Condition")]
        [SerializeField] private AtomBaseVariable variable;
        [SerializeField] private AtomCondition[] conditions;
        [SerializeField] private AtomConditionOperators operators;
        
        [Header("Event")]
        [SerializeField] private UnityEvent<bool> onCondition;
        [SerializeField] private UnityEvent onTrue;
        [SerializeField] private UnityEvent onFalse;
    }
}