UPDATE draw_no SET 
    thousand_no=1, hundred_no=2, ten_no=3, one_no=4,ball5=5,
    draw_datetime=Now(),period_status=3,draw_count=draw_count+1,is_checkout=1,show_frontend=1
    WHERE period_no=20170826002;
DELETE FROM draw_no_detail WHERE period_no = 20170826002; 
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 6, 'XX34');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 4, 'X2X4');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 5, 'X23X');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 3, '1XX4');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 2, '1X3X');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 1, '12XX');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 10, 'X234');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 9, '1X34');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 8, '12X4');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 7, '123X');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 11, '1234');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 15, 'XXX45');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 12, '12');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 12, '13');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 12, '14');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 12, '23');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 12, '24');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 12, '34');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 13, '123');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 13, '124');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 13, '134');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 13, '234');
INSERT INTO draw_no_detail(period_no, dict_no_type_id, bet_no) VALUES(20170826002, 14, '1234');
UPDATE bet_20170826 SET win_money = 0 WHERE period_no=20170826002 AND win_money > 0 ORDER BY bet_id;
UPDATE bet_20170826 SET win_money = bet_money * odds WHERE bet_id IN (-1) ;
UPDATE draw_win_detail SET is_obsolete = 1 WHERE period_no = 20170826002 AND draw_no <> 12345 ORDER BY member_id,period_no,draw_no;
INSERT INTO draw_win_detail (member_id,period_no,draw_no,bet_money,win_money,profit_money)
    SELECT member_id,20170826002 ,12345, SUM(bet_money), SUM(win_money) AS period_win_money,
    SUM(win_money) + SUM(bet_money * return_water_rate) - SUM(bet_money)
    FROM bet_20170826
    WHERE period_no = 20170826002 AND member_id IN (-1) AND is_cancel = 0
    GROUP BY member_id
ON DUPLICATE KEY UPDATE
    is_obsolete=0;
UPDATE sell_sum_20170826 SET is_win = 0 WHERE period_no=20170826002 AND is_win=1 ORDER BY period_no,member_id,bet_no,dict_no_type_id;
UPDATE sell_sum_20170826 SET if_profit_money=0 WHERE period_no=20170826002 AND bet_no='XXXX' ORDER BY period_no,member_id,bet_no,dict_no_type_id;
 UPDATE sell_sum_20170826 SET is_win = 1 WHERE period_no=20170826002 AND  (bet_no IN ('XX34','X2X4','X23X','1XX4','1X3X','12XX','X234','1X34','12X4','123X','1234','XXX45','12','13','14','23','24','34','123','124','134','234'))  ORDER BY period_no,member_id,bet_no,dict_no_type_id;
UPDATE sell_sum_20170826 SET if_profit_money=(SELECT SUM(if_win_money * is_win - bet_money) FROM (SELECT * FROM sell_sum_20170826 WHERE bet_no<>'XXXX' AND period_no=20170826002) AS view_table ) WHERE bet_no='XXXX' AND period_no=20170826002;
DELETE FROM sell_type_sum_20170825 WHERE period_no = 20170826002;
INSERT INTO sell_type_sum_20170825 (period_no,dict_no_type_id,bet_money,profit_money) 
    SELECT 20170826002 AS 'period_no', dict_no_type_id, SUM(bet_money) AS 'bet_money', SUM(bet_money - if_win_money * is_win) AS 'profit_money' FROM sell_sum_20170825 WHERE period_no = 20170826002 AND dict_no_type_id<>999 GROUP BY dict_no_type_id, member_id;
INSERT INTO sell_type_sum_20170825 (period_no,dict_no_type_id,bet_money,profit_money) 
    SELECT a.period_no,9920 AS 'dict_no_type_id',SUM(bet_money) AS 'bet_money',SUM(profit_money) AS 'profit_money' FROM sell_type_sum_20170825 AS a
    JOIN dict_no_type AS b
    ON a.dict_no_type_id=b.dict_no_type_id AND b.fix_num=20 AND a.period_no=20170826002
    GROUP BY a.period_no
    UNION ALL
    SELECT a.period_no,9930 AS 'dict_no_type_id',SUM(bet_money) AS 'bet_money',SUM(profit_money) AS 'profit_money' FROM sell_type_sum_20170825 AS a
    JOIN dict_no_type AS b
    ON a.dict_no_type_id=b.dict_no_type_id AND b.fix_num=30 AND a.period_no=20170826002
    GROUP BY a.period_no
    UNION ALL
    SELECT a.period_no,999 AS 'dict_no_type_id',SUM(bet_money) AS 'bet_money',SUM(profit_money) AS 'profit_money' FROM sell_type_sum_20170825 AS a 
    WHERE dict_no_type_id not in (999,9920,9930) AND
    a.period_no=20170826002
    GROUP BY a.period_no;
UPDATE async_contribution_20170826 SET hold_win_money = 0 WHERE period_no=20170826002 ORDER BY period_no,banker_id,child_id;
UPDATE async_contribution_sum_20170826 SET hold_win_money_amount = 0 WHERE period_no=20170826002 ORDER BY period_no,banker_id;
UPDATE async_ledger_20170826 SET win_money = 0, hold_win_money = 0 WHERE period_no=20170826002 ORDER BY period_no,member_id,dict_no_type_id;
UPDATE async_report_20170826 SET member_win_money = 0, lower_level_hold_win_money_amount = 0, hold_win_money = 0 WHERE period_no=20170826002 ORDER BY period_no,banker_id,child_id;
UPDATE member AS a JOIN (
        SELECT member_id, SUM(win_money) AS 'total_win_money' 
        FROM bet_20170826
        WHERE bet_id IN (-1)
        GROUP BY member_id) AS b ON a.member_id = b.member_id
    SET a.win_money_all = a.win_money_all + b.total_win_money, a.credit_balance = a.credit_balance + b.total_win_money;
INSERT INTO async_ledger_sum_20170825(parent_id,member_id,dict_no_type_id,bet_count,bet_money,return_water,win_money,hold_count,hold_money,hold_return_water,hold_win_money)
        SELECT parent_id,member_id,dict_no_type_id,bet_count,bet_money,return_water,win_money,hold_count,hold_money,hold_return_water,hold_win_money FROM async_ledger_20170826 AS t WHERE period_no=20170826002
          ON DUPLICATE KEY UPDATE 
            bet_count=async_ledger_sum_20170825.bet_count+t.bet_count,
            bet_money=async_ledger_sum_20170825.bet_money+t.bet_money,
            return_water=async_ledger_sum_20170825.return_water+t.return_water,
            win_money=async_ledger_sum_20170825.win_money+t.win_money,
            hold_count=async_ledger_sum_20170825.hold_count+t.hold_count,
            hold_money=async_ledger_sum_20170825.hold_money+t.hold_money,
            hold_return_water=async_ledger_sum_20170825.hold_return_water+t.hold_return_water,
            hold_win_money=async_ledger_sum_20170825.hold_win_money+t.hold_win_money;
INSERT INTO async_contribution_day_sum_20170825  (
        banker_id,
        child_id,
        hold_money,
        hold_profit_loss_money,
        hold_win_money,
        contribution_money
    ) 
    SELECT 
        banker_id,
        child_id,
        hold_money,
        hold_profit_loss_money,
        hold_win_money,
        contribution_money
    FROM async_contribution_20170826 B
    WHERE period_no = 20170826002
    ON DUPLICATE KEY UPDATE
    async_contribution_day_sum_20170825.hold_money = async_contribution_day_sum_20170825.hold_money+B.hold_money,
    async_contribution_day_sum_20170825.hold_profit_loss_money = async_contribution_day_sum_20170825.hold_profit_loss_money + B.hold_profit_loss_money,
    async_contribution_day_sum_20170825.hold_win_money = async_contribution_day_sum_20170825.hold_win_money + B.hold_win_money,
    async_contribution_day_sum_20170825.contribution_money = async_contribution_day_sum_20170825.contribution_money + B.contribution_money;
INSERT INTO async_report_sum_20170825(banker_id,child_id,member_bet_count,member_bet_money,child_return_water,member_profit_loss_money,member_win_money,lower_level_contribution_money,lower_level_hold_win_money_amount,lower_level_profit_loss_money_amount,hold_money,hold_return_water,hold_profit_loss_money,hold_profit_loss_money_amount,hold_win_money,higher_level_contribution_money)
    SELECT banker_id,child_id,member_bet_count,member_bet_money,child_return_water,member_profit_loss_money,member_win_money,lower_level_contribution_money,lower_level_hold_win_money_amount,lower_level_profit_loss_money_amount,hold_money,hold_return_water,hold_profit_loss_money,hold_profit_loss_money_amount,hold_win_money,higher_level_contribution_money FROM async_report_20170826 AS t WHERE period_no=20170826002
    ON DUPLICATE KEY UPDATE
        member_bet_count=async_report_sum_20170825.member_bet_count+t.member_bet_count,
        member_bet_money=async_report_sum_20170825.member_bet_money+t.member_bet_money,
        child_return_water=async_report_sum_20170825.child_return_water+t.child_return_water,
        member_profit_loss_money=async_report_sum_20170825.member_profit_loss_money+t.member_profit_loss_money,
        member_win_money=async_report_sum_20170825.member_win_money+t.member_win_money,
        lower_level_contribution_money=async_report_sum_20170825.lower_level_contribution_money+t.lower_level_contribution_money,
        lower_level_hold_win_money_amount=async_report_sum_20170825.lower_level_hold_win_money_amount+t.lower_level_hold_win_money_amount,
        lower_level_profit_loss_money_amount=async_report_sum_20170825.lower_level_profit_loss_money_amount+t.lower_level_profit_loss_money_amount,
        hold_money=async_report_sum_20170825.hold_money+t.hold_money,
        hold_return_water=async_report_sum_20170825.hold_return_water+t.hold_return_water,
        hold_profit_loss_money=async_report_sum_20170825.hold_profit_loss_money+t.hold_profit_loss_money,
        hold_profit_loss_money_amount=async_report_sum_20170825.hold_profit_loss_money_amount+t.hold_profit_loss_money_amount,
        hold_win_money=async_report_sum_20170825.hold_win_money+t.hold_win_money,
        higher_level_contribution_money=async_report_sum_20170825.higher_level_contribution_money+t.higher_level_contribution_money;
UPDATE draw_no AS t1 JOIN
    (
        SELECT period_no,
        CASE WHEN IFNULL(member_bet_count,0) > 0 THEN CEIL(ROUND(IFNULL(member_bet_count,0),3)) ELSE FLOOR(ROUND(IFNULL(member_bet_count,0),3)) END AS bet_count,
        CASE WHEN IFNULL(hold_total_profit_loss_money-sell_profit_loss_money,0) > 0 THEN CEIL(ROUND(IFNULL(hold_total_profit_loss_money-sell_profit_loss_money,0),3)) ELSE FLOOR(ROUND(IFNULL(hold_total_profit_loss_money-sell_profit_loss_money,0),3)) END AS director_profit_loss_money
        FROM(
        SELECT 20170826002 AS period_no,
        SUM(IFNULL(A.member_bet_count,0)) AS member_bet_count, -- 会员总投笔数
        SUM(A.hold_profit_loss_money_amount-A.hold_win_money) AS hold_total_profit_loss_money, -- 本级总盈亏,
        (SELECT (IFNULL(a.bet_money,0)-IFNULL(b.profit_money,0)) AS profit_money FROM 
            (SELECT IFNULL(SUM(bet_money),0) AS bet_money FROM sell_sum_20170826 WHERE period_no=20170826002) a
            JOIN 
            (SELECT IFNULL(SUM(if_win_money),0) AS profit_money FROM sell_sum_20170826 WHERE period_no=20170826002 AND is_win=1) b ) AS sell_profit_loss_money	-- 出货盈亏
        FROM async_report_20170826 A INNER JOIN member B
            ON A.child_id  = B.member_id
        WHERE A.period_no=20170826002 AND A.banker_id = (SELECT member_id FROM member WHERE member_level=6 AND is_sub_account=0 LIMIT 1)
        ) AS a
    )  AS t2
    ON t1.period_no=t2.period_no
    SET t1.bet_count=t2.bet_count,t1.director_profit_loss_money=t2.director_profit_loss_money 
    WHERE t1.period_no=20170826002;