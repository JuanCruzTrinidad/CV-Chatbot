// import logo from './logo.svg';
// import './App.css';

// function App() {
//   return (
//     <div className="App">
//       <header className="App-header">
//         <img src={logo} className="App-logo" alt="logo" />
//         <p>
//           Edit <code>src/App.js</code> and save to reload.
//         </p>
//         <a
//           className="App-link"
//           href="https://reactjs.org"
//           target="_blank"
//           rel="noopener noreferrer"
//         >
//           Learn React
//         </a>
//       </header>
//     </div>
//   );
// }

// export default App;

import React, { useMemo } from 'react';
import ReactWebChat, { createDirectLine } from 'botframework-webchat';

export default () => {
  //SAk8gCB-UP0.6V61TZ4vbGsehTCApa2AukRYfEIg9Tue2vyyGoB_mNY
  const directLine = useMemo(() => createDirectLine({ token: 'ew0KICAiYWxnIjogIlJTMjU2IiwNCiAgImtpZCI6ICJsY0oxTXFpNkdKYXdCZEw5Y0dieEt5S1R6OE0iLA0KICAieDV0IjogImxjSjFNcWk2R0phd0JkTDljR2J4S3lLVHo4TSIsDQogICJ0eXAiOiAiSldUIg0KfQ.ew0KICAiYm90IjogImp1YW50cmluaWRhZCIsDQogICJzaXRlIjogIjNjWnByUDdtaHZZIiwNCiAgImNvbnYiOiAiRXhwVnk2RkNFR1JCQzgzUUJJSXlhZC0zIiwNCiAgInVzZXIiOiAiNzNkZWExYjktNTM3Mi00YTQ0LTg3MTgtZWQzYmIxNzNjOTEwIiwNCiAgIm9yaWdpbiI6ICJodHRwczovL3dlYmNoYXQuYm90ZnJhbWV3b3JrLmNvbS8iLA0KICAibmJmIjogMTYxOTU1NDk3MywNCiAgImV4cCI6IDE2MTk1NTg1NzMsDQogICJpc3MiOiAiaHR0cHM6Ly93ZWJjaGF0LmJvdGZyYW1ld29yay5jb20vIiwNCiAgImF1ZCI6ICJodHRwczovL3dlYmNoYXQuYm90ZnJhbWV3b3JrLmNvbS8iDQp9.TqT9DwxRcutAhp1aHS46Q6LcfeVOTz7gk8NiF5YzLWo8UvRDZiAmijFH2iCtl6MuUxhMcn7kab59bEZHlBfZCCuvyuawnNHa5-Fty0FFCl8td4SstMg7YTw8giP40tqnU3VhHTrptdfMnWkfYCnj7a9uxfvlra6ABhJ_LiWYPlxFbj0k5HIOjASUqlH2bQ8ldeblzNyuwB_736xc5hfA-h_2zZ4CDWi6mZU5fuGxhQGv-0FISd0z-qSR3D94v8HlrA4jyQVFCZQOVxibFT4rvjAAIAX7E3X08KGIQdQhokWbvELcB1eCUOUZfxjNTtGGSI64bMLAK6tW5lQUQB5XVg' }), []);

  return <ReactWebChat directLine={directLine} userID="Juan C" />;
};