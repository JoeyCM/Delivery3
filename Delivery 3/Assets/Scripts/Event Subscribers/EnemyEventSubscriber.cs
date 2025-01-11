using UnityEngine;

namespace Gamekit3D
{
    public class EnemyEventSubscriber : MonoBehaviour
    {
        [SerializeField] private Damageable enemyDamageable;

        private void OnEnable()
        {
            if (enemyDamageable != null)
            {
                // Subscribe to the UnityEvents
                enemyDamageable.OnDeath.AddListener(SendOnDeathData);
            }
        }

        private void OnDisable()
        {
            if (enemyDamageable != null)
            {
                // Unsubscribe from the UnityEvents
                enemyDamageable.OnDeath.RemoveListener(SendOnDeathData);
            }
        }

        private void SendOnDeathData()
        {
            // Collect and send damage event data
            Vector3 position = transform.position;
            string positionString = position.ToString();

            EnemyEventData data = new EnemyEventData
            {
                EnemyEventType = "OnDeath",
                EnemyTimestamp = System.DateTime.UtcNow.ToString("o"),
                EnemyPosition = positionString
            };

            Debug.Log("Enemy Death Event at Position: " + positionString);
            SendDataToServer(data);
        }

        private void SendDataToServer(EnemyEventData data)
        {
            string jsonData = JsonUtility.ToJson(data);
            StartCoroutine(ServerSender.SendData(jsonData));
        }
    }

    [System.Serializable]
    public class EnemyEventData
    {
        public string EnemyEventType;
        public string EnemyTimestamp;
        public string EnemyPosition;
    }
}
