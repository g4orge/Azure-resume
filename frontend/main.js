const functionApiUrl = 'https://getresumecounter-dev.azurewebsites.net/api/GetResumeCounter?code=WnCLQWxv6xa8JXES9V0yjahu_aghG-5zfCBG51l0KMWuAzFuhTFHtA==';

const getVisitCount = () => {
    fetch(functionApiUrl)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP ${response.status} - ${response.statusText}`);
            }
            return response.json();
        })
        .then(data => {
            console.log("Website called function API.");
            const count = data.count;
                if (count === undefined) throw new Error("API response missing 'count'");
                    document.getElementById("counter").innerText = count;
              
        })
        .catch(error => {
            console.error("Error fetching visit count:", error);
            document.getElementById("counter").innerText = "Error";
        });
};

window.addEventListener('DOMContentLoaded', getVisitCount);