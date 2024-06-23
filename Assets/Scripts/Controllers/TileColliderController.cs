using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileColliderController : MonoBehaviour
{
    void Start()
    {
        // Definir as camadas
        int playerLayer = LayerMask.NameToLayer("Default");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int boxLayer = LayerMask.NameToLayer("Box");

        // Desativar a colisão entre player/enemy e a caixa inicialmente
        Physics2D.IgnoreLayerCollision(playerLayer, boxLayer, false);
        Physics2D.IgnoreLayerCollision(enemyLayer, boxLayer, false);
    }

    void OnCollisionEnter2D(Collision2D _collision)
    {
        // Verifica se a colisão é com o jogador ou inimigo
        if (_collision.gameObject.layer == LayerMask.NameToLayer("Default") ||
            _collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Verifica se o ponto de contato está acima do centro da caixa
            Vector2 contactPoint = _collision.GetContact(0).point;
            Vector2 boxTop = new Vector2(transform.position.x, transform.position.y + transform.localScale.y / 2);

            if (contactPoint.y >= boxTop.y)
            {
                // Ativar colisão entre o objeto e a caixa
                Physics2D.IgnoreLayerCollision(_collision.gameObject.layer, LayerMask.NameToLayer("Box"), false);
            }
            else
            {
                Physics2D.IgnoreLayerCollision(_collision.gameObject.layer, LayerMask.NameToLayer("Box"), true);
            }
        }
    }

    void OnCollisionExit2D(Collision2D _collision)
    {
        // Verifica se a colisão é com o jogador ou inimigo
        if (_collision.gameObject.layer == LayerMask.NameToLayer("Default") ||
            _collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Desativar colisão entre o objeto e a caixa
            Physics2D.IgnoreLayerCollision(_collision.gameObject.layer, LayerMask.NameToLayer("Box"), true);
        }
    }
}

