using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody),typeof(BoxCollider))]
public class KarakterKontrol : MonoBehaviour
{

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private Animator _animator;

    [SerializeField] private float _movespeed;

    private Camera cam;
    private Animator animator;
    public Material m_material;

    public float turnSpeed, speed, lerpValue;
    public LayerMask layer;

    public Transform toplanacaklarAO;
    public GameObject prevObject;
    public List<GameObject> toplanank = new List<GameObject>();

    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
    }

//Hareket
    void FixedUpdate(){
        JoystickMovement();
    }

    private void JoystickMovement(){
        _rigidbody.velocity = new Vector3(_joystick.Horizontal * _movespeed, _rigidbody.velocity.y, _joystick.Vertical * _movespeed);

        if(_joystick.Horizontal != 0 || _joystick.Vertical != 0){
            transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
            _animator.SetBool("running",true);
        }
        else{
            _animator.SetBool("running",false);
        }
    }

    private void Movement(){
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.transform.localPosition.z;

        Ray ray = cam.ScreenPointToRay(mousePos);

        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,Mathf.Infinity,layer)){
            //transform.DOMove(hit.point,lerpValue);
            Vector3 hitVec = hit.point;
            hitVec.y = transform.position.y;

            transform.position = Vector3.MoveTowards(transform.position, Vector3.Lerp(transform.position, hitVec, lerpValue), Time.deltaTime * speed);
            Vector3 newMovePoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newMovePoint - transform.position), turnSpeed * Time.deltaTime);

            if(!animator.GetBool("running")){
                animator.SetBool("running",true);
            }
        }
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

            target.tag = "Untagged";

        }
   
//&& target.gameObject.tag == "DizK"
    if (toplanank.Count > 1 && target.gameObject.tag != "Diz" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0,1) && target.gameObject.tag.StartsWith("Diz")){
  
         GameObject obje = toplanank[toplanank.Count - 1];
         toplanank.RemoveAt(toplanank.Count - 1);
         Destroy(obje);

        // m_material = target.GetComponent<MeshRenderer>().material;
         target.GetComponent<MeshRenderer>().material = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material;
         target.GetComponent<MeshRenderer>().enabled = true;

         target.tag = "Diz" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0,1);

         prevObject = toplanank[toplanank.Count - 1];

        }   
    }
 }
