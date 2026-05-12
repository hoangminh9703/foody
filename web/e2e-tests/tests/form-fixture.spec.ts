import { test, expect } from '@playwright/test';

test.describe('Form interaction fixture', () => {
  test('fills and submits a form with validation', async ({ page }) => {
    await page.setContent(`
      <!doctype html>
      <html lang="en">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1" />
          <title>Form Fixture</title>
          <style>
            body { font-family: Arial, sans-serif; padding: 24px; }
            form { display: grid; gap: 12px; max-width: 420px; }
            label { display: grid; gap: 6px; }
            input, textarea, button { padding: 10px 12px; font-size: 16px; }
            .error { color: #b91c1c; min-height: 1.25rem; }
            .success { color: #047857; margin-top: 12px; }
          </style>
        </head>
        <body>
          <h1>Contact form</h1>
          <form id="contact-form">
            <label>
              Name
              <input id="name" name="name" type="text" required />
            </label>
            <label>
              Email
              <input id="email" name="email" type="email" required />
            </label>
            <label>
              Message
              <textarea id="message" name="message" rows="4" required></textarea>
            </label>
            <button type="submit">Send message</button>
            <div id="error" class="error" role="alert"></div>
            <div id="success" class="success" aria-live="polite"></div>
          </form>
          <script>
            const form = document.getElementById('contact-form');
            const error = document.getElementById('error');
            const success = document.getElementById('success');
            form.addEventListener('submit', (event) => {
              event.preventDefault();
              error.textContent = '';
              success.textContent = '';
              const name = document.getElementById('name').value.trim();
              const email = document.getElementById('email').value.trim();
              const message = document.getElementById('message').value.trim();
              if (!name || !email || !message) {
                error.textContent = 'All fields are required';
                return;
              }
              success.textContent = 'Thanks, ' + name + '. We will reply to ' + email + '.';
            });
          </script>
        </body>
      </html>
    `);

    await expect(page.getByRole('heading', { name: /contact form/i })).toBeVisible();

    await page.getByLabel('Name').fill('Nguyen Van A');
    await page.getByLabel('Email').fill('a@example.com');
    await page.getByLabel('Message').fill('I would like to place an order.');
    await page.getByRole('button', { name: /send message/i }).click();

    await expect(page.getByText(/thanks, nguyen van a/i)).toBeVisible();
    await expect(page.getByText(/we will reply to a@example.com/i)).toBeVisible();
  });
});