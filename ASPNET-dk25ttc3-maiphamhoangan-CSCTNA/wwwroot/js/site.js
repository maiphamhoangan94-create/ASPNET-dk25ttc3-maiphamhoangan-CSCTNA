document.addEventListener('DOMContentLoaded', function () {

    const navbar = document.querySelector('.navbar');
    window.addEventListener('scroll', function () {
        if (window.scrollY > 50) {
            navbar.classList.add('shadow-sm', 'bg-white');
            navbar.classList.remove('bg-warm');
        } else {
            navbar.classList.remove('shadow-sm', 'bg-white');
            navbar.classList.add('bg-warm');
        }
    });

    const statsSection = document.querySelector('.bg-warm');
    const counters = document.querySelectorAll('.display-6');
    let animated = false;

    function startCounter() {
        if (!statsSection) return;
        const sectionPos = statsSection.getBoundingClientRect().top;
        const screenPos = window.innerHeight / 1.2;

        if (sectionPos < screenPos && !animated) {
            counters.forEach(counter => {
                const text = counter.innerText.trim();
                const target = parseFloat(text.replace(/[^0-9.]/g, ''));
                const suffix = text.replace(/[0-9.]/g, '');
                let count = 0;
                const speed = target / 50;

                const updateCount = () => {
                    count += speed;
                    if (count < target) {
                        if (target % 1 !== 0) {
                            counter.innerText = count.toFixed(1) + suffix;
                        } else {
                            counter.innerText = Math.ceil(count).toLocaleString() + suffix;
                        }
                        setTimeout(updateCount, 30);
                    } else {
                        counter.innerText = text; 
                    }
                };
                updateCount();
            });
            animated = true;
        }
    }
    window.addEventListener('scroll', startCounter);

    const cards = document.querySelectorAll('.card-recipe, .card-taste');

    cards.forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        card.style.transition = 'all 0.6s ease-out';
    });

    const revealOnScroll = function () {
        cards.forEach(card => {
            const cardTop = card.getBoundingClientRect().top;
            const triggerBottom = window.innerHeight * 0.85;

            if (cardTop < triggerBottom) {
                card.style.opacity = '1';
                card.style.transform = 'translateY(0)';
            }
        });
    };

    window.addEventListener('scroll', revealOnScroll);
    revealOnScroll();

    const newsletterBtn = document.querySelector('section form button');
    const newsletterInput = document.querySelector('section form input');

    if (newsletterBtn && newsletterInput) {
        newsletterBtn.addEventListener('click', function (e) {
            e.preventDefault();
            const email = newsletterInput.value.trim();
            if (email === '' || !email.includes('@')) {
                alert('Vui lòng nhập địa chỉ email hợp lệ!');
                newsletterInput.focus();
            } else {
                alert('Cảm ơn bạn đã đăng ký nhận bản tin Bếp Nhà!');
                newsletterInput.value = '';
            }
        });
    }
});