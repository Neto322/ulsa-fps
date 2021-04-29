using System.Collections;
using System.Collections.Generic;
using MLAPI;   
using MLAPI.NetworkVariable;
using UnityEngine;

public class Health_Component : NetworkBehaviour
{
    [SerializeField]
        private NetworkVariableInt m_Health = new NetworkVariableInt(100);

        public NetworkVariableInt Health => m_Health;

        void OnEnable()
        {
            // Subscribe for when Health value changes
            // This usually get's triggered when the server modifies that variable
            // and is later replicated down to clients
            Health.OnValueChanged += OnHealthChanged;

            Debug.Log("Spawned tu salud ahora  " + Health.Value);
        }

        void OnDisable()
        {
            Health.OnValueChanged -= OnHealthChanged;
        }

        void OnHealthChanged(int oldValue, int newValue)
        {
            // Update UI, if this a client instance and it's the owner of the object
            if (IsOwner && IsClient)
            {
                // TODO: Update UI code?
            }

            Debug.LogFormat("{0} has {1} health!", gameObject.name, m_Health.Value);
        }

        public void TakeDamage(int amount)
        {
            // Health should be modified server-side only
            if(!IsServer) return;
            Health.Value -= amount;

        

            if (Health.Value <= 0)
            {
                Health.Value = 0;
            }
        }
}
