using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine.UI;

public class KarakterAlgoritması : MonoBehaviour
{
    public GameObject targetsParent;
    public List<GameObject> targets = new List<GameObject>();
    public float radius=2;
    public Transform toplanacaklarAO;
    public GameObject prevObject;
    public List<GameObject> toplanank = new List<GameObject>();
    public Transform[] fuels;
    public Material m_material;

    private Animator animator;
    private NavMeshAgent agent;
    private bool haveTarget = false;
    private Vector3 targetTransform;

    void Start()
    {
        for (int i = 0; i<targetsParent.transform.childCount; i++){
            targets.Add(targetsParent.transform.GetChild(i).gameObject);
        }      
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();  
    }

   
    void Update()
    {
        if(!haveTarget && targets.Count>0){
            ChooseTarget();
        }
    }

    void ChooseTarget(){

        int randomNumber = Random.Range(0,3);

        if (randomNumber == 0 && toplanank.Count >= 6){
            int randomFuel = Random.Range(0,fuels.Length);
            List<Transform> fuelsNonActiveChild = new List<Transform>();
            foreach(Transform item in fuels[randomFuel])
            {
                if(!item.GetComponent<MeshRenderer>().enabled || item.GetComponent<MeshRenderer>().enabled && item.gameObject.tag != "Diz" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0,1)){
                    fuelsNonActiveChild.Add(item);    
                }
            }

            targetTransform = toplanank.Count > fuelsNonActiveChild.Count ? fuelsNonActiveChild[fuelsNonActiveChild.Count -1].position : fuelsNonActiveChild[toplanank.Count].position;
        }
        else{

        Collider[] hitTopla = Physics.OverlapSphere(transform.position,radius);
        List<Vector3> ourColors = new List<Vector3>();
        for (int i=0; i<hitTopla.Length; i++){
            if (hitTopla[i].tag.StartsWith(transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0,1))){
                ourColors.Add(hitTopla[i].transform.position);
            }
        }
        if (ourColors.Count>0){
            targetTransform = ourColors[0];
        }
        else{
            int random = Random.Range(0,targets.Count);
            targetTransform = targets[random].transform.position;
        }
    }    
        agent.SetDestination(targetTransform);
        if(!animator.GetBool("running")){
            animator.SetBool("running",true);
        }
        haveTarget = true;
    
}

    private void OnTriggerEnter(Collider target){
        if(target.gameObject.tag.StartsWith(transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0,1))){
            target.transform.SetParent(toplanacaklarAO);
            Vector3 pos = prevObject.transform.localPosition;

            pos.y += 0.22f;
            pos.z = 0;
            pos.x = 0;

            target.transform.localRotation = new Quaternion(0,0.7071068f,0,0.7071068f);

            target.transform.DOLocalMove(pos,0.2f);
            prevObject = target.gameObject;
            toplanank.Add(target.gameObject);

            targets.Remove(target.gameObject);
            target.tag = "Untagged";
            haveTarget = false;

//target.gameObject.tag == "DizK" ||

        } else if ( target.gameObject.tag != "Diz" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0,1) && target.gameObject.tag.StartsWith("Diz")){
            if (toplanank.Count > 1){
                GameObject obje = toplanank[toplanank.Count - 1];
                toplanank.RemoveAt(toplanank.Count - 1);
                Destroy(obje);

               // m_material = target.GetComponent<MeshRenderer>().material;

                target.GetComponent<MeshRenderer>().material = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material;
                target.GetComponent<MeshRenderer>().enabled = true;

                target.tag = "Diz" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0,1);

            }else{
                prevObject = toplanank[0].gameObject;
                haveTarget = false;
            }
        }
    
    }

    private void OnDrawGizmos(){
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
