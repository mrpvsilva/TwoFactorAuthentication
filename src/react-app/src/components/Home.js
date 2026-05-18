import React from 'react';

export default function Home() {
  return (
    <div className="space-y-4 py-4">
      <h1 className="text-3xl font-bold">Hello, world!</h1>
      <p>Welcome to your new single-page application, built with:</p>
      <ul className="list-disc list-inside space-y-1">
        <li>
          <a className="text-primary underline" href='https://get.asp.net/'>ASP.NET Core</a> and{' '}
          <a className="text-primary underline" href='https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx'>C#</a>{' '}
          for cross-platform server-side code
        </li>
        <li><a className="text-primary underline" href='https://facebook.github.io/react/'>React</a> for client-side code</li>
        <li><a className="text-primary underline" href='http://tailwindcss.com/'>Tailwind CSS</a> for layout and styling</li>
      </ul>
      <p>To help you get started, we have also set up:</p>
      <ul className="list-disc list-inside space-y-1">
        <li><strong>Client-side navigation</strong>. For example, click <em>Fetch data</em> then <em>Back</em> to return here.</li>
        <li><strong>Development server integration</strong>. In development mode, the development server from <code>create-react-app</code> runs in the background automatically, so client-side resources are dynamically built on demand and the page refreshes when you modify any file.</li>
        <li><strong>Efficient production builds</strong>. In production mode, development-time features are disabled, and your <code>dotnet publish</code> configuration produces minified, efficiently bundled JavaScript files.</li>
      </ul>
    </div>
  );
}
