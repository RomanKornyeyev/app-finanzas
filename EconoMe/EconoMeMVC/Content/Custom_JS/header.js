var toggle = document.getElementById("nav-toggle");
var headerContent = document.getElementById("header-content");
var barTop = document.querySelector(".bar--top");
var barMiddle = document.querySelector(".bar--middle");
var barBottom = document.querySelector(".bar--bottom");

//al hacer click al trigger, se despliega el menú
toggle.addEventListener("click", function(){
    //añadir la clase con el height al header__content
    headerContent.classList.toggle("height-calc-100-header");

    //animación del menú hamburguesa (3 barras)
    barTop.classList.toggle("bar--top-efect");
    barMiddle.classList.toggle("bar--middle-efect");
    barBottom.classList.toggle("bar--bottom-efect");

    //desabilitar scroll al desplegar el menú
    if (window.onscroll == null) {
        var x = window.scrollX;
        var y = window.scrollY;
        window.onscroll = function(){ window.scrollTo(x, y) };
    }else{
        window.onscroll = null;
    }
});

//si el menú sigue desplegado al superar el punto de ruptura, se pliega
window.addEventListener("resize", function(){
    //si el menú sigue desplegado al superar el punto de ruptura, se pliega
    if (window.innerWidth > 810) {
        if (headerContent.classList.contains("height-calc-100-header")) {
            headerContent.classList.remove("height-calc-100-header");
        }

        //reset del menú hamburguesa (3 barras) al resize
        barTop.classList.remove("bar--top-efect");
        barMiddle.classList.remove("bar--middle-efect");
        barBottom.classList.remove("bar--bottom-efect");

        //por si acaso si el escroll sigue bloqueado al superar el punto de ruptura, lo desbloqueamos
        window.onscroll = null;
    }
});