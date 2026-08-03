using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SurvivalEngine;

namespace Suriyun.MCS.Example
{
    public class Ability : MonoBehaviour
    {
        /* 

        boolean for each effect

        Effects:
        Damage
              array for collider
              oncollide function
        Summon
              instanciation

        */

        public bool isDamage, isSummon;


        public float rotationSpeed;

        public float size0;
        public float sizeDecaySpeed;

        protected float boxHeight;

        //public GameObject summonPrefab;
        public CraftData summonPrefab;

        private void Start()
        {
            this.transform.localScale = Vector3.one * size0;
            if (isDamage)
            {
                StartCoroutine("DamageRoutine");
            }

            if (isSummon)
            {
                StartCoroutine("SummonRoutine");

                //Instantiate(summonPrefab, this.transform.position, Quaternion.identity);
            }
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            transform.localScale = transform.localScale - Vector3.one * sizeDecaySpeed * Time.deltaTime;

            boxHeight = transform.localScale.y / 2f;

            transform.position = new Vector3(
                transform.position.x,
                boxHeight,
                transform.position.z
                );

            if (transform.localScale.x < 0.01f)
            {
                Destroy(this.gameObject);
            }
        }


        #region Effects
        public enum DamageType
        {
            Physical,

            Magic,

        }

        public Collider area;

        public DamageType damageType;

        public int damage;

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Enemy")
            {
                AttackTarget target = other.GetComponent<AttackTarget>();
                target.TakeDamage(damage);
            }
        }

        public IEnumerator DamageRoutine()
        {
            yield return new WaitForSeconds(0.2f);
            area.enabled = false;
        }

        public IEnumerator SummonRoutine()
        {
            yield return new WaitForSeconds(0.2f);
            PlayerCharacter character = PlayerCharacter.Get();
            GameObject summon = CraftData.Create(summonPrefab, this.transform.position);
            Pet pet = summon.GetComponent<Pet>();
            pet.TamePet(character);
        }
        #endregion
    }
}