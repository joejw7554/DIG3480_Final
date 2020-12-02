using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardEnemyController : MonoBehaviour
  {

    private RubyController rubyController;//

    public float speed=3.0f;
    public bool vertical;
    public float changeTime=3.0f;

    public ParticleSystem smokeEffect;

    Rigidbody2D Rigidbody2D;
    float timer;
    int direction= 1;
    bool broken= true;

    Animator animator;

    public AudioClip playerhitsound;
    public AudioClip fixedsound;
    AudioSource audioSource;

    public GameObject lootdrop;
    
    void Start()
    {
        Rigidbody2D= GetComponent<Rigidbody2D>();
        timer= changeTime;
        animator=GetComponent<Animator>();
        audioSource= GetComponent<AudioSource>();

        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");//

        if (rubyControllerObject != null)//

        {

            rubyController = rubyControllerObject.GetComponent<RubyController>(); //and this line of code finds the rubyController and then stores it in a variable

            print ("Found the RubyConroller Script!");//

        }

        if (rubyController == null)//

        {

            print ("Cannot find GameController Script!");//

        }
    }

    void Update()
    {
          if(!broken)
        {
            return;
        }

        timer -= Time.deltaTime;
        
        if (timer < 0)
        {
            direction= -direction;
            timer=changeTime;
        }
    }

    void FixedUpdate()
    {
           if(!broken)
        {
            return;
        }

        Vector2 position= Rigidbody2D.position;

        if (vertical)
        {
            position.y= position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }

        else
        {
            position.x=position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        Rigidbody2D.MovePosition(position);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player!=null)
        {
            player.ChangeHealth(-2);
        }
    }

    public void Fix()
    {
        broken=false;
        Rigidbody2D.simulated= false;
        animator.SetTrigger("Fixed");
        rubyController.ChangeScore(1);


        smokeEffect.Stop();
        PlaySound(fixedsound);
        Instantiate(lootdrop,transform.position, Quaternion.identity);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

}

