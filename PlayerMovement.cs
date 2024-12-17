using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public bool isGrounded;
    public float circleCollideRadius = 0.3f;
    public LayerMask groundLayer;
    public Transform respawnPoint;
    public Transform kickPoint;
    public Transform jumpKickPoint;
    public float kickRange = 0.5f;
    public float jumpKickRange = 0.5f;
    public LayerMask enemyLayer;
    public float deathHeight = -15;
    public int stock;
    public bool canKick = true;
    public bool canMove = true;
    public int kickDamage = 20;

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D feetCollider;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector3 faceRight;
    private Vector3 faceLeft;

    // Start is called before the first frame update
    void Start()
    {
        // Get the SpriteRenderer component attached to the GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        //Get circle collider
        feetCollider = GetComponentInChildren<CircleCollider2D>();
        //Get Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        //Get Animator
        anim = GetComponent<Animator>();

        faceRight = new Vector3(3f, 3f, 3f);
        faceLeft = new Vector3(-3f, 3f, 3f);
        setStock(stock);
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
        groundCheck();

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (!isGrounded)
            {
                JumpKick();
            }
            else
            {
                Kick();
            }
        }

        if (transform.position.y < deathHeight)
        {
            RespawnPlayer();
        }
    }

    void Move()
    {
        if (canMove)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            Vector2 moveDirection = new Vector2(moveX, 0f);
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

            // Flip the sprite when moving left or right
            if (moveX < 0)  // Moving left
            {
                gameObject.transform.localScale = faceLeft;
            }
            else if (moveX > 0)  // Moving right
            {
                gameObject.transform.localScale = faceRight;
            }
            anim.SetFloat("speed", Mathf.Abs(moveX));
        }
    }

    void groundCheck()
    {
        // Cast a circle at the feet's position to check for ground
        isGrounded = Physics2D.OverlapCircle(feetCollider.bounds.center, feetCollider.radius + circleCollideRadius, groundLayer);
        if (!isGrounded)
        {
            anim.SetBool("onGround", false);
        }
        else
        {
            anim.SetBool("onGround", true);
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            // Use the cached Rigidbody2D reference to apply the jump force
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetTrigger("jump");
        }
    }

    void Kick()
    {
        if ((isGrounded) && (canKick))
        {
            anim.SetTrigger("kick");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(kickPoint.position, kickRange, enemyLayer);

            foreach(Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Enemy>().TakeDamage(kickDamage);
                Debug.Log("Enemy hit");
            }
        }
    }

    void JumpKick()
    {
        if (!isGrounded)
        {
            anim.SetTrigger("jumpKick");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(jumpKickPoint.position, jumpKickRange, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Enemy>().TakeDamage(kickDamage);
                Debug.Log("Enemy hit");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(kickPoint.position, kickRange);
        Gizmos.DrawWireSphere(jumpKickPoint.position, jumpKickRange);
    }

    void CanKickTrue()
    {
        canKick = true;
        canMove = true;
    }

    void CanKickFalse()
    {
        canKick = false;
        canMove = false;
    }

    void RespawnPlayer()
    {
        // Teleport the player to the respawn point
        transform.position = respawnPoint.position;

        // Optionally reset velocity if you want to stop any falling or momentum
        rb.velocity = Vector2.zero;
        stockRemove();
    }

    void setStock(int stock)
    {
        this.stock = stock;
    }

    void stockRemove()
    {
        this.stock--;
    }

    void stockPlus()
    {
        this.stock++;
    }
}
