window.onload = function(){
    document.onclick = function(e){
        if(e.target.className != "dropdown"){
            document.getElementsByClassName("dropdown-content")[0].style.display = null;
        }
    };
};

function ShowDropdown(){
    document.getElementsByClassName("dropdown-content")[0].style.display = "block";
}