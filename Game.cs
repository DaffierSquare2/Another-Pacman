﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnotherPacman
{
    public partial class Game : Form
    {
        private int initialEnemyCount = 1;

        private Random rand = new Random();
        private Level level = new Level();
        private Hero hero = new Hero();
        private Food food = new Food();
        private Timer mainTimer = null;
        private Timer enemySpawningTimer = null;
        private List<Enemy> enemies = new List<Enemy>();

        public Game()
        {
            InitializeComponent();
            InitializeGame();
            InitializeMainTimer();
            HeroBorderCollision();
            InitializeEnemySpawningTimer();
        }

        private void MoveHero()
        {
            hero.Left += hero.HorizontalVelocity;
            hero.Top += hero.VerticalVelocity;
        }

        private void MoveEnemies()
        {
            foreach(var enemy in enemies)
            {
                enemy.Left += enemy.HorizontalVelocity;
                enemy.Top += enemy.VerticalVelocity;
            }
        }

        private void InitializeGame()
        {
            //adjust game form size
            this.Size = new Size(630, 650);
            //add key down event handler
            this.KeyDown += Game_KeyDown;

            AddLevel();
            AddHero();
            AddEnemies(initialEnemyCount);
            AddFood();
        }

        private void AddFood()
        {
            this.Controls.Add(food);
            food.Parent = level;
            food.BringToFront();
        }

        private void AddLevel()
        {
            //adding level to the game
            this.Controls.Add(level);
        }

        private void AddHero()
        {
            //adding Hero to the game
            this.Controls.Add(hero);
            hero.Parent = level;
            hero.BringToFront();
        }
        private void InitializeMainTimer()
        {
            mainTimer = new Timer();
            mainTimer.Tick += MainTimer_Tick;
            mainTimer.Interval = 50;
            mainTimer.Start();
        }

        private void InitializeEnemySpawningTimer()
        {
            enemySpawningTimer = new Timer();
            enemySpawningTimer.Tick += EnemySpawningTimer_Tick;
            enemySpawningTimer.Interval = 3000;
            enemySpawningTimer.Start();
        }

        private void EnemySpawningTimer_Tick(object sender, EventArgs e)
        {
            AddEnemies(1);
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            MoveHero();
            MoveEnemies();
            HeroBorderCollision();
            EnemyBorderCollision();
            HeroEnemyColission();
            HeroFoodCollision();
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D:
                    hero.Direction = "right";
                    hero.HorizontalVelocity = hero.Step;
                    hero.VerticalVelocity = 0;
                    break;
                case Keys.S:
                    hero.Direction = "down";
                    hero.HorizontalVelocity = 0;
                    hero.VerticalVelocity = hero.Step;
                    break;
                case Keys.A:
                    hero.Direction = "left";
                    hero.HorizontalVelocity = -hero.Step;
                    hero.VerticalVelocity = 0;
                    break;
                case Keys.W:
                    hero.Direction = "up";
                    hero.HorizontalVelocity = 0;
                    hero.VerticalVelocity = -hero.Step;
                    break;
            }
            SetRandomEnemyDirection();
        }
        private void HeroBorderCollision()
        {
            if (hero.Left > level.Left + level.Width)
            {
                hero.Left = level.Left - hero.Width;
            }
            if (hero.Left + hero.Width < level.Left)
            {
                hero.Left = level.Left + level.Width;
            }
            if (hero.Top > level.Top + level.Height)
            {
                hero.Top = level.Top - hero.Height;
            }
            if (hero.Top + hero.Height < level.Top)
            {
                hero.Top = level.Top + level.Height;
            }
        }

        private void EnemyBorderCollision()
        {
            foreach (var enemy in enemies)
            {
                if (enemy.Top < level.Top) //From "up" to "down"
                {
                    enemy.SetDirection(2);
                }
                if (enemy.Top > level.Height - enemy.Width) //From "down" to "up"
                {
                    enemy.SetDirection(4);
                }
                if (enemy.Left < level.Left) //From "left" to "right"
                {
                    enemy.SetDirection(1);
                }
                if (enemy.Left > level.Width - enemy.Width) //From "right" to "left"
                {
                    enemy.SetDirection(3);
                }
            }
        }

        private void HeroFoodCollision()
        {
            if (hero.Bounds.IntersectsWith(food.Bounds))
            {
                hero.Step += 1;
                RespawnFood();
            }
        }

        private void RespawnFood()
        {
            food.Location = new Point(rand.Next(0, 550), rand.Next(0, 550));
            food.SetType(rand.Next(1, 5));
        }

        private void AddEnemies(int enemyCount)
        {
            Enemy enemy;
            for (int i = 0; i < enemyCount; i++)
            {
                enemy = new Enemy();
                enemy.Location = new Point(rand.Next(0, 550), rand.Next(0, 550));
                enemy.SetDirection(rand.Next(1, 5));
                enemies.Add(enemy);
                this.Controls.Add(enemy);
                enemy.Parent = level;
                enemy.BringToFront();
            }
        }

        private void SetRandomEnemyDirection()
        {
            foreach(var enemy in enemies)
            {
                enemy.SetDirection(rand.Next(1, 5));
            }
        }

        private void GameOver()
        {
            mainTimer.Stop();
            GameOverLabel.BackColor = Color.Transparent;
            GameOverLabel.Parent = level;
            GameOverLabel.Visible = true;
            GameOverLabel.BringToFront();
        }

        private void HeroEnemyColission()
        {
            foreach (var enemy in enemies)
            {
                if (enemy.Bounds.IntersectsWith(hero.Bounds))
                {
                    GameOver();
                }
            }
        }
    }
}