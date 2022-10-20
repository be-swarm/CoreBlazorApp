
export function setTheme(isLihtTheme) {
    document.getElementsByTagName('body')[0].style.display = 'none';
    let applink = document.getElementById('apptheme');
    applink.href = isLihtTheme ? '_content/BeSwarm.CoreBlazorApp/css/theme-light.css' : '_content/BeSwarm.CoreBlazorApp/css/theme-dark.css';
 
    setTimeout(function () { document.getElementsByTagName('body')[0].style.display = 'block'; }, 200);
}







