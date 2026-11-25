using UnityEngine;

namespace HorrorSchool.Core
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private string spawnID;
        public string SpawnID => spawnID;
    }
}