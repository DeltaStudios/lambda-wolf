using UnityEngine;
using System.Collections;

public interface IDamagable{

	void DamageMe(float damage, Vector2 force); 
	void DamageMe(Vector2 force); 

}
