using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{   

    
    Collider2D m_Collider;//test

    public bool gameOver1=false;
    public float speed=3.0f;
    public static int level=1;

    private int score=0;
    public Text cointext;
    public Text conditionaltext;
    public Text scoretext;
    public Text cogstext;

    public int coins=0;
    public int cogs=4;
    public int maxHealth =5;

    public GameObject projectilePrefab;
    public GameObject HealthDecreasePrefab;
    public GameObject HealthIncreasePrefab;

    public AudioClip powerupsound;
    public AudioClip missioncomplete;
    public AudioClip npcinteraction;
    public AudioClip backgroundmusic;
    public AudioClip victorymusic;
    public AudioClip lossmusic;
    public AudioClip cogpickupsoundeffect;
    public AudioClip coinpickupsoundeffect;


    public AudioSource musicsource;

    public AudioClip throwSound;
    public AudioClip hitSound;

    public int health { get {return currentHealth;}}
    int currentHealth;

    public float timeInvincible=2.0f;

    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    private float poweruptimer;
    private bool powerup;

    Animator animator;
    Vector2 lookDirection= new Vector2(1,0);

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    { 
        m_Collider = GetComponent<Collider2D>();
        Scoretext();
        Cointext();
        musicsource.clip=backgroundmusic;
        musicsource.Play();
        
        poweruptimer=0;
        powerup=false;

    
        rigidbody2d= GetComponent<Rigidbody2D>();
        animator= GetComponent<Animator>();

        currentHealth= maxHealth;
   
        audioSource= GetComponent<AudioSource>();
        conditionaltext.text="";
        cogstext.text="cogs:"+ cogs.ToString();
    }


    // Update is called once per frame
    void Update()
    {  
        if (currentHealth==0)
        {
            if (Input.GetKey(KeyCode.R) & level==1)
            {
            SceneManager.LoadScene("Stage1");
            }

            if (Input.GetKey(KeyCode.R) & level==2)
            {
            SceneManager.LoadScene("Stage2");
            }
        }

        Scoretext();
        Cointext();

        horizontal= Input.GetAxis("Horizontal");
        vertical= Input.GetAxis("Vertical");

        Vector2 move= new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y,0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if(invincibleTimer < 0)
                isInvincible= false;
        }

         if(Input.GetKeyDown(KeyCode.C))
        {
            if(cogs>0)
            {
            Launch();
            cogs=cogs-1;
            cogstext.text="cogs:"+cogs.ToString();
            }
        }

            if(score==4 & level==1)
            {
                conditionaltext.text="Talk to Jambi to visit stage 2";
            
                RaycastHit2D hit= Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider !=null)
                {
                     if (Input.GetKeyDown(KeyCode.X))
                        {   
                            SceneManager.LoadScene("Stage2");
                            level++;
                        }
                     
                }
            }    

            if (Input.GetKeyDown(KeyCode.X))
                {
                    RaycastHit2D hit= Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));

                    if (hit.collider !=null)
                        {
                            NonPlayerCharacter character= hit.collider.GetComponent<NonPlayerCharacter>();

                        if(character != null)
                            {
                                if(score<4 | coins<4)
                                {
                                PlaySound(npcinteraction);
                                character.DisplayDialog();
                                }

                                else if(score==4 & level==1)
                                {
                                    character.DisplayDialog3();
                                }

                                else if(coins==4 & level==2)
                                {
                                character.DisplayDialog2();
                                conditionaltext.text="You Win! Game Created by JaeWooJoe. Press R to restart";
                                musicsource.clip=victorymusic;
                                musicsource.Play();
                                speed=0;
                                coins=5;
                                level=3;
                                
                            
                                }
                                
                            }
                        }
                }

            if (Input.GetKeyDown(KeyCode.R) && level==3)//Restart for winning
                    {
                        SceneManager.LoadScene("Stage2");
                        level=2;
                    }

            if (Input.GetKeyDown(KeyCode.R))
            {
            RaycastHit2D hit= Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if(hit.collider!=null & coins==4)
            {
            SceneManager.LoadScene("Stage2");
            }
            }


            if (Input.GetKey("escape"))
            {
            Application.Quit();
            }


            if(powerup)
            {
            poweruptimer+=Time.deltaTime;

                if (poweruptimer>=2)
                {
                    speed=3;
                    poweruptimer=0;
                    powerup=false;
                }
            }
    }

    void FixedUpdate()
    {
        Vector2 position= rigidbody2d.position;
        position.x= position.x+ speed* horizontal * Time.deltaTime;
        position.y= position.y+ speed* vertical * Time.deltaTime;
        
        rigidbody2d.MovePosition(position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag=="powerup")
            {
                powerup=true;
                speed+=2f;
                PlaySound(powerupsound);
                Destroy(other.gameObject);
            }
    }


    public void ChangeHealth(int amount)
    {
        if (amount <0)
        {
            if (isInvincible)
                return;

                isInvincible=true;
                invincibleTimer= timeInvincible;

                GameObject HealthDecrease= Instantiate(HealthDecreasePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);//
                animator.SetTrigger("Hit");
                PlaySound(hitSound);          
        }
        
        if (amount>0)
        {
                GameObject HealthIncrease= Instantiate(HealthIncreasePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);//
        }

        currentHealth= Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (currentHealth==0)
        {
            conditionaltext.text="You lost! Press R to restart";
            speed=0;
            gameOver1=true;
                if (gameOver1)
                {
                Defeatcondition();
                m_Collider.enabled = !m_Collider.enabled;
                    if (Input.GetKey(KeyCode.R))
                    {
                    SceneManager.LoadScene("Stage1");
                    }
                }
        }


    }

    public void ChangeScore(int scoreamount)
    {
        score= score+scoreamount;
    }

    void Launch()
    {
        GameObject projectileObject= Instantiate(projectilePrefab,rigidbody2d.position + Vector2.up * 1.0f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }

      public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void ChangeCoin(int coinquantity)
    {
        coins=coins+coinquantity;
        PlaySound(coinpickupsoundeffect);

         if (coins==4)
            {
            conditionaltext.text="Talk to Jambi";
            }
    }

    public void Cointext()
    {
        if(level==2)
        {
        cointext.text="coins:"+coins.ToString();
        }
    }
    public void Scoretext()
    {
        if(level==1)
        {
        scoretext.text="Score:"+score.ToString();
        }
        else
        {
        scoretext.text="";
        }
    }

    public void ChangeCog (int cogamount)
    {
        cogs=cogs+cogamount;
        cogstext.text="cogs:"+cogs.ToString();
        PlaySound(cogpickupsoundeffect);
    }

    public void Defeatcondition ()
    {
        musicsource.clip= lossmusic;
        musicsource.Play();
    }

            
    
}