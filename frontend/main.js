window.addEventListener('DOMContentLoaded', (event) => {
    getVIsitCount();
});
const functionApi = '';

const getVIsitCount = () => {
    let count = 30;
    fetch(functionApi).then(response => {
        return response.json();
    }).then(response => {
        count = response.count;
        console.log(count);
        document.getElementById('counter').innerText = count;
    }).catch(function(error) {
        console.log('Error:', error);
    })
    return count;
}
    